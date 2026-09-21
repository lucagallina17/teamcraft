using TeamCraft.Application.DTOs.Team;
using TeamCraft.Application.Interfaces.Repositories;
using TeamCraft.Application.Repositories;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.Services.Implementations;

public class TeamMatchingService : ITeamMatchingService
{
    private const double AffinityWeightFactor = 0.1;
    private const int ProposalsCount = 3;

    // Limite di sicurezza sui nodi esplorati dal backtracking — evita tempi di calcolo
    // eccessivi in scenari con un numero di candidati eleggibili molto alto per slot.
    // Con team HR realistici (poche decine di candidati per ruolo) non si avvicina mai a questo limite.
    private const int MaxSearchNodes = 200_000;

    private readonly IProjectRepository _projectRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeAffinityRepository _affinityRepository;
    private readonly ITeamReadRepository _teamReadRepository;

    public TeamMatchingService(
        IProjectRepository projectRepository,
        IEmployeeRepository employeeRepository,
        IEmployeeAffinityRepository affinityRepository,
        ITeamReadRepository teamReadRepository
        )
    {
        _projectRepository = projectRepository;
        _employeeRepository = employeeRepository;
        _affinityRepository = affinityRepository;
        _teamReadRepository = teamReadRepository;
    }

    public async Task<List<TeamProposalDto>> GenerateProposalsAsync(Guid projectId)
    {
        var project = await _projectRepository.GetByIdWithRequirementsAsync(projectId);
        if (project == null) return new List<TeamProposalDto>();

        var employees = (await _employeeRepository.GetAllWithCompetenciesAsync()).ToList();

        // Un dipendente già impegnato in un team Active su qualunque progetto
        // non è disponibile come candidato — esclusione a monte, prima dello scoring
        var busyEmployeeIds = (await _teamReadRepository.GetEmployeeIdsInActiveTeamsAsync()).ToHashSet();
        employees = employees.Where(e => !busyEmployeeIds.Contains(e.Id)).ToList();

        var slots = project.ProjectRoleRequirements
            .SelectMany(req => Enumerable.Range(0, req.Quantity).Select(_ => req))
            .ToList();

        var candidatesByRequirement = project.ProjectRoleRequirements.ToDictionary(
            req => req.Id,
            req => ScoreCandidates(req, employees)
        );

        // Precarichiamo TUTTE le affinità rilevanti in una sola query prima della ricerca,
        // così il backtracking lavora interamente in memoria senza round-trip al database
        // ad ogni nodo esplorato — necessario per rendere la ricerca praticabile in tempi ragionevoli
        var allCandidateIds = candidatesByRequirement.Values
            .SelectMany(c => c.Select(x => x.EmployeeId))
            .Distinct()
            .ToList();

        var affinities = await _affinityRepository.GetForEmployeesAsync(allCandidateIds);
        var affinityLookup = affinities.ToDictionary(
            a => NormalizePairKey(a.EmployeeId1, a.EmployeeId2),
            a => a.Score
        );

        var proposals = new List<TeamProposalDto>();
        var excludedEmployeeIds = new HashSet<Guid>();

        for (int i = 0; i < ProposalsCount; i++)
        {
            var assignment = FindOptimalAssignment(slots, candidatesByRequirement, excludedEmployeeIds, affinityLookup);
            if (assignment.Count == 0) break;

            var affinityBonus = CalculateAffinityBonus(assignment.Select(a => a.EmployeeId), affinityLookup);
            var competencyTotal = assignment.Sum(a => a.CompetencyScore);

            proposals.Add(new TeamProposalDto
            {
                ProposalId = Guid.NewGuid(),
                TotalScore = Math.Round(competencyTotal + affinityBonus, 2),
                IsComplete = assignment.Count == slots.Count,
                Members = assignment.Select(a => new TeamProposalMemberDto
                {
                    EmployeeId = a.EmployeeId,
                    FullName = a.FullName,
                    ProjectRoleRequirementId = a.RequirementId,
                    ProjectRoleId = a.ProjectRoleId,
                    RoleName = a.RoleName,
                    CompetencyScore = Math.Round(a.CompetencyScore, 2)
                }).ToList()
            });

            // Per generare 3 proposte realmente diverse, escludiamo il candidato che ha contribuito
            // di più al punteggio della soluzione ottima appena trovata, forzando la ricerca successiva
            // a trovare la migliore composizione ALTERNATIVA senza quella persona
            var topContributor = assignment.OrderByDescending(a => a.CompetencyScore).FirstOrDefault();
            if (topContributor != null) excludedEmployeeIds.Add(topContributor.EmployeeId);
        }

        return proposals.OrderByDescending(p => p.TotalScore).ToList();
    }

    public async Task<double> RecalculateScoreAsync(
        Guid projectId,
        List<(Guid EmployeeId, Guid ProjectRoleRequirementId)> assignments)
    {
        var project = await _projectRepository.GetByIdWithRequirementsAsync(projectId);
        if (project == null) return 0;

        var employeeIds = assignments.Select(a => a.EmployeeId).ToList();
        var employees = (await _employeeRepository.GetAllWithCompetenciesAsync())
            .Where(e => employeeIds.Contains(e.Id))
            .ToList();

        double competencyTotal = 0;

        foreach (var (employeeId, requirementId) in assignments)
        {
            var requirement = project.ProjectRoleRequirements.FirstOrDefault(r => r.Id == requirementId);
            var employee = employees.FirstOrDefault(e => e.Id == employeeId);
            if (requirement == null || employee == null) continue;

            competencyTotal += CalculateCompetencyScore(requirement, employee).score;
        }

        var affinities = await _affinityRepository.GetForEmployeesAsync(employeeIds);
        var affinityLookup = affinities.ToDictionary(
            a => NormalizePairKey(a.EmployeeId1, a.EmployeeId2),
            a => a.Score
        );

        var affinityBonus = CalculateAffinityBonus(employeeIds, affinityLookup);

        return Math.Round(competencyTotal + affinityBonus, 2);
    }

    // ============ RICERCA DELL'ASSEGNAMENTO OTTIMO (backtracking con potatura) ============

    private List<EmployeeCandidate> FindOptimalAssignment(
        List<ProjectRoleRequirement> slots,
        Dictionary<Guid, List<EmployeeCandidate>> candidatesByRequirement,
        HashSet<Guid> excludedEmployeeIds,
        Dictionary<string, int> affinityLookup)
    {
        // Processiamo prima gli slot più "scarsi" (meno candidati eleggibili) — non solo
        // per correttezza (evita di sacrificare uno slot senza alternative, vedi [046]),
        // ma anche perché è la strategia che pota l'albero di ricerca più rapidamente
        var orderedSlots = slots
            .Select((req, index) => new
            {
                Index = index,
                Requirement = req,
                Candidates = candidatesByRequirement[req.Id]
                    .Where(c => !excludedEmployeeIds.Contains(c.EmployeeId))
                    .ToList()
            })
            .OrderBy(s => s.Candidates.Count)
            .ToList();

        // Limite superiore teorico per la potatura: se anche assegnando il candidato
        // migliore possibile a ogni slot rimanente non si supera la miglior soluzione
        // trovata finora, quel ramo di ricerca viene abbandonato senza esplorarlo
        var maxPossiblePerSlot = orderedSlots
            .Select(s => s.Candidates.Count > 0 ? s.Candidates.Max(c => c.CompetencyScore) : 0)
            .ToList();

        EmployeeCandidate?[] bestAssignment = new EmployeeCandidate?[orderedSlots.Count];
        EmployeeCandidate?[] currentAssignment = new EmployeeCandidate?[orderedSlots.Count];
        int bestFilledCount = -1;
        double bestScore = -1;
        int nodesVisited = 0;

        void Backtrack(int slotPosition, HashSet<Guid> usedEmployees, double currentScore, int filledCount)
        {
            nodesVisited++;
            if (nodesVisited > MaxSearchNodes) return; // safety net per scenari anomali

            if (slotPosition == orderedSlots.Count)
            {
                // Preferiamo sempre più slot riempiti; a parità di slot riempiti, il punteggio più alto
                // Nota: l'affinità va ricalcolata sull'insieme completo, non sommata incrementalmente,
                // perché dipende dalla combinazione finale di persone, non dalle singole assegnazioni
                var affinityBonus = CalculateAffinityBonus(usedEmployees, affinityLookup);
                var totalWithAffinity = currentScore + affinityBonus;

                if (filledCount > bestFilledCount ||
                    (filledCount == bestFilledCount && totalWithAffinity > bestScore))
                {
                    bestFilledCount = filledCount;
                    bestScore = totalWithAffinity;
                    Array.Copy(currentAssignment, bestAssignment, currentAssignment.Length);
                }
                return;
            }

            // Limite superiore ottimistico per il ramo corrente: somma dei migliori punteggi
            // possibili per tutti gli slot non ancora processati, ignorando l'affinità (piccola,
            // impatto marginale sulla potatura) per mantenere il calcolo semplice e veloce
            double optimisticRemaining = 0;
            for (int i = slotPosition; i < orderedSlots.Count; i++)
                optimisticRemaining += maxPossiblePerSlot[i];

            var optimisticTotal = currentScore + optimisticRemaining;
            var maxPossibleFilled = filledCount + (orderedSlots.Count - slotPosition);

            // Pota il ramo se non può battere la miglior soluzione già trovata,
            // né per completezza né per punteggio
            if (maxPossibleFilled < bestFilledCount) return;
            if (maxPossibleFilled == bestFilledCount && optimisticTotal <= bestScore && bestFilledCount >= 0) return;

            var slot = orderedSlots[slotPosition];

            // Opzione A: prova ad assegnare ciascun candidato eleggibile a questo slot
            foreach (var candidate in slot.Candidates)
            {
                if (usedEmployees.Contains(candidate.EmployeeId)) continue;
                if (nodesVisited > MaxSearchNodes) return;

                currentAssignment[slotPosition] = candidate;
                usedEmployees.Add(candidate.EmployeeId);

                Backtrack(slotPosition + 1, usedEmployees, currentScore + candidate.CompetencyScore, filledCount + 1);

                usedEmployees.Remove(candidate.EmployeeId);
                currentAssignment[slotPosition] = null;
            }

            // Opzione B: lascia lo slot vuoto e prosegui — necessario per gestire il caso
            // in cui non riempirlo permetta una composizione complessiva migliore o più completa altrove
            currentAssignment[slotPosition] = null;
            Backtrack(slotPosition + 1, usedEmployees, currentScore, filledCount);
        }

        Backtrack(0, new HashSet<Guid>(), 0, 0);

        return bestAssignment.Where(a => a != null).Select(a => a!).ToList();
    }

    private static double CalculateAffinityBonus(IEnumerable<Guid> employeeIds, Dictionary<string, int> affinityLookup)
    {
        var ids = employeeIds.ToList();
        if (ids.Count < 2) return 0;

        var scores = new List<int>();
        for (int i = 0; i < ids.Count; i++)
        {
            for (int j = i + 1; j < ids.Count; j++)
            {
                var key = NormalizePairKey(ids[i], ids[j]);
                if (affinityLookup.TryGetValue(key, out var score))
                    scores.Add(score);
            }
        }

        if (scores.Count == 0) return 0;

        return scores.Average() * AffinityWeightFactor;
    }

    private static string NormalizePairKey(Guid id1, Guid id2)
    {
        return id1.CompareTo(id2) < 0 ? $"{id1}_{id2}" : $"{id2}_{id1}";
    }

    // ============ SCORING DEI CANDIDATI PER SLOT (invariato da prima) ============

    private List<EmployeeCandidate> ScoreCandidates(ProjectRoleRequirement requirement, List<Employee> employees)
    {
        var candidates = new List<EmployeeCandidate>();

        foreach (var employee in employees)
        {
            var (isEligible, score) = CalculateCompetencyScore(requirement, employee);
            if (!isEligible) continue;

            candidates.Add(new EmployeeCandidate
            {
                EmployeeId = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                RequirementId = requirement.Id,
                ProjectRoleId = requirement.ProjectRoleId,
                RoleName = requirement.ProjectRole.Name,
                CompetencyScore = score
            });
        }

        return candidates
            .OrderByDescending(c => c.CompetencyScore)
            .ThenBy(c => c.EmployeeId)
            .ToList();
    }

    private (bool isEligible, double score) CalculateCompetencyScore(ProjectRoleRequirement requirement, Employee employee)
    {
        bool isEligible = true;
        double score = 0;

        foreach (var reqComp in requirement.RequirementCompetencies)
        {
            var assessment = employee.CompetencyAssessments
                .FirstOrDefault(a => a.CompetencyId == reqComp.CompetencyId);

            var level = assessment?.Level ?? 0;

            if (reqComp.RequirementType == RequirementType.Required && level < reqComp.MinimumLevel)
            {
                isEligible = false;
                continue;
            }

            if (reqComp.MinimumLevel > 0 && level >= reqComp.MinimumLevel)
            {
                var ratio = Math.Min(level / (double)reqComp.MinimumLevel, 1.5);
                score += ratio * reqComp.Weight;
            }
        }

        return (isEligible, score);
    }

    private class EmployeeCandidate
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public Guid RequirementId { get; set; }
        public Guid ProjectRoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public double CompetencyScore { get; set; }
    }
}