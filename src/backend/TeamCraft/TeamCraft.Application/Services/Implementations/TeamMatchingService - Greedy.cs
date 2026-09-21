//using TeamCraft.Application.DTOs.Team;
//using TeamCraft.Application.Interfaces.Repositories;
//using TeamCraft.Application.Services.Interfaces;
//using TeamCraft.Application.Repositories;
//using TeamCraft.Domain.Entities;
//using TeamCraft.Domain.Enums;

//namespace TeamCraft.Application.Services.Implementations;

//public class TeamMatchingService : ITeamMatchingService
//{
//    // Affinità come "bonus/malus leggero" — non deve mai superare il peso delle competenze
//    private const double AffinityWeightFactor = 0.1;
//    private const int ProposalsCount = 3;

//    private readonly IProjectRepository _projectRepository;
//    private readonly IEmployeeRepository _employeeRepository;
//    private readonly IEmployeeAffinityRepository _affinityRepository;

//    public TeamMatchingService(
//        IProjectRepository projectRepository,
//        IEmployeeRepository employeeRepository,
//        IEmployeeAffinityRepository affinityRepository)
//    {
//        _projectRepository = projectRepository;
//        _employeeRepository = employeeRepository;
//        _affinityRepository = affinityRepository;
//    }

//    public async Task<List<TeamProposalDto>> GenerateProposalsAsync(Guid projectId)
//    {
//        var project = await _projectRepository.GetByIdWithRequirementsAsync(projectId);
//        if (project == null) return new List<TeamProposalDto>();

//        var employees = (await _employeeRepository.GetAllWithCompetenciesAsync()).ToList();

//        // Ogni requirement con Quantity > 1 genera più "slot" da riempire singolarmente
//        var slots = project.ProjectRoleRequirements
//            .SelectMany(req => Enumerable.Range(0, req.Quantity).Select(_ => req))
//            .ToList();

//        var candidatesByRequirement = project.ProjectRoleRequirements.ToDictionary(
//            req => req.Id,
//            req => ScoreCandidates(req, employees)
//        );

//        var proposals = new List<TeamProposalDto>();
//        var excludedEmployeeIds = new HashSet<Guid>();

//        for (int i = 0; i < ProposalsCount; i++)
//        {
//            var assignment = GreedyAssign(slots, candidatesByRequirement, excludedEmployeeIds);
//            if (assignment.Count == 0) break;

//            var affinityBonus = await CalculateAffinityBonusAsync(assignment.Select(a => a.EmployeeId));
//            var competencyTotal = assignment.Sum(a => a.CompetencyScore);

//            proposals.Add(new TeamProposalDto
//            {
//                ProposalId = Guid.NewGuid(),
//                TotalScore = Math.Round(competencyTotal + affinityBonus, 2),
//                IsComplete = assignment.Count == slots.Count,
//                Members = assignment.Select(a => new TeamProposalMemberDto
//                {
//                    EmployeeId = a.EmployeeId,
//                    FullName = a.FullName,
//                    ProjectRoleRequirementId = a.RequirementId,
//                    ProjectRoleId = a.ProjectRoleId,
//                    RoleName = a.RoleName,
//                    CompetencyScore = Math.Round(a.CompetencyScore, 2)
//                }).ToList()
//            });

//            // Escludiamo il candidato più forte in assoluto per forzare una composizione
//            // di team realmente alternativa nella proposta successiva, non una variante marginale
//            var topEmployee = assignment.OrderByDescending(a => a.CompetencyScore).FirstOrDefault();
//            if (topEmployee != null) excludedEmployeeIds.Add(topEmployee.EmployeeId);
//        }

//        return proposals.OrderByDescending(p => p.TotalScore).ToList();
//    }

//    public async Task<double> RecalculateScoreAsync(
//        Guid projectId,
//        List<(Guid EmployeeId, Guid ProjectRoleRequirementId)> assignments)
//    {
//        var project = await _projectRepository.GetByIdWithRequirementsAsync(projectId);
//        if (project == null) return 0;

//        var employeeIds = assignments.Select(a => a.EmployeeId).ToList();
//        var employees = (await _employeeRepository.GetAllWithCompetenciesAsync())
//            .Where(e => employeeIds.Contains(e.Id))
//            .ToList();

//        double competencyTotal = 0;

//        foreach (var (employeeId, requirementId) in assignments)
//        {
//            var requirement = project.ProjectRoleRequirements.FirstOrDefault(r => r.Id == requirementId);
//            var employee = employees.FirstOrDefault(e => e.Id == employeeId);
//            if (requirement == null || employee == null) continue;

//            competencyTotal += CalculateCompetencyScore(requirement, employee).score;
//        }

//        var affinityBonus = await CalculateAffinityBonusAsync(employeeIds);

//        return Math.Round(competencyTotal + affinityBonus, 2);
//    }

//    private List<EmployeeCandidate> ScoreCandidates(ProjectRoleRequirement requirement, List<Employee> employees)
//    {
//        var candidates = new List<EmployeeCandidate>();

//        foreach (var employee in employees)
//        {
//            var (isEligible, score) = CalculateCompetencyScore(requirement, employee);
//            if (!isEligible) continue;

//            candidates.Add(new EmployeeCandidate
//            {
//                EmployeeId = employee.Id,
//                FullName = $"{employee.FirstName} {employee.LastName}",
//                RequirementId = requirement.Id,
//                ProjectRoleId = requirement.ProjectRoleId,
//                RoleName = requirement.ProjectRole.Name,
//                CompetencyScore = score
//            });
//        }

//        return candidates
//            .OrderByDescending(c => c.CompetencyScore)
//            .ThenBy(c => c.EmployeeId) // tie-break deterministico
//            .ToList();
//    }

//    private (bool isEligible, double score) CalculateCompetencyScore(ProjectRoleRequirement requirement, Employee employee)
//    {
//        bool isEligible = true;
//        double score = 0;

//        foreach (var reqComp in requirement.RequirementCompetencies)
//        {
//            var assessment = employee.CompetencyAssessments
//                .FirstOrDefault(a => a.CompetencyId == reqComp.CompetencyId);

//            var level = assessment?.Level ?? 0;

//            // Una competenza Required non soddisfatta esclude subito il candidato dal ruolo
//            if (reqComp.RequirementType == RequirementType.Required && level < reqComp.MinimumLevel)
//            {
//                isEligible = false;
//                continue;
//            }

//            if (reqComp.MinimumLevel > 0 && level >= reqComp.MinimumLevel)
//            {
//                var ratio = Math.Min(level / (double)reqComp.MinimumLevel, 1.5);
//                score += ratio * reqComp.Weight;
//            }
//        }

//        return (isEligible, score);
//    }

//    private List<EmployeeCandidate> GreedyAssign(
//        List<ProjectRoleRequirement> slots,
//        Dictionary<Guid, List<EmployeeCandidate>> candidatesByRequirement,
//        HashSet<Guid> excludedEmployeeIds)
//    {
//        var assignedEmployeeIds = new HashSet<Guid>();
//        var filledSlotIndexes = new HashSet<int>();
//        var result = new List<EmployeeCandidate>();

//        // Ordiniamo gli SLOT per scarsità di candidati (meno alternative = priorità più alta),
//        // non solo le coppie per punteggio — questo evita che un candidato "jolly" con punteggio
//        // pari su più ruoli venga sprecato su un ruolo che aveva comunque altre alternative valide
//        var orderedSlotIndexes = slots
//            .Select((req, index) => new
//            {
//                Index = index,
//                EligibleCount = candidatesByRequirement[req.Id]
//                    .Count(c => !excludedEmployeeIds.Contains(c.EmployeeId))
//            })
//            .OrderBy(s => s.EligibleCount)
//            .Select(s => s.Index)
//            .ToList();

//        foreach (var slotIndex in orderedSlotIndexes)
//        {
//            if (filledSlotIndexes.Contains(slotIndex)) continue;

//            var requirement = slots[slotIndex];

//            var bestCandidate = candidatesByRequirement[requirement.Id]
//                .Where(c => !excludedEmployeeIds.Contains(c.EmployeeId))
//                .Where(c => !assignedEmployeeIds.Contains(c.EmployeeId))
//                .OrderByDescending(c => c.CompetencyScore)
//                .ThenBy(c => c.EmployeeId) // tie-break deterministico residuo
//                .FirstOrDefault();

//            if (bestCandidate == null) continue; // nessun candidato idoneo rimasto per questo slot

//            result.Add(bestCandidate);
//            filledSlotIndexes.Add(slotIndex);
//            assignedEmployeeIds.Add(bestCandidate.EmployeeId);
//        }

//        return result;
//    }

//    private async Task<double> CalculateAffinityBonusAsync(IEnumerable<Guid> employeeIds)
//    {
//        var ids = employeeIds.ToList();
//        if (ids.Count < 2) return 0;

//        var affinities = (await _affinityRepository.GetForEmployeesAsync(ids)).ToList();
//        if (affinities.Count == 0) return 0;

//        var averageAffinity = affinities.Average(a => a.Score);
//        return averageAffinity * AffinityWeightFactor;
//    }

//    private class EmployeeCandidate
//    {
//        public Guid EmployeeId { get; set; }
//        public string FullName { get; set; } = string.Empty;
//        public Guid RequirementId { get; set; }
//        public Guid ProjectRoleId { get; set; }
//        public string RoleName { get; set; } = string.Empty;
//        public double CompetencyScore { get; set; }
//    }
//}