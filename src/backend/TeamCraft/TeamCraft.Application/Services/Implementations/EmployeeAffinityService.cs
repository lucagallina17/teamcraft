using TeamCraft.Application.DTOs.Employee;
using TeamCraft.Application.Interfaces.Repositories;
using TeamCraft.Application.Repositories;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.Services.Implementations;

public class EmployeeAffinityService : IEmployeeAffinityService
{
    private readonly IEmployeeAffinityRepository _affinityRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeAffinityService(
        IEmployeeAffinityRepository affinityRepository,
        IEmployeeRepository employeeRepository)
    {
        _affinityRepository = affinityRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<ColleaguePairDto>> GetColleaguesWithAffinityAsync()
    {
        var pairs = await _affinityRepository.GetColleaguePairsAsync();
        var employees = (await _employeeRepository.GetAllAsync()).ToDictionary(e => e.Id);

        // Una sola query per tutte le affinità esistenti, invece di una query per coppia
        var allEmployeeIds = pairs.SelectMany(p => new[] { p.EmployeeId1, p.EmployeeId2 }).Distinct();
        var allAffinities = await _affinityRepository.GetForEmployeesAsync(allEmployeeIds);

        var affinityLookup = allAffinities.ToDictionary(
            a => NormalizePairKey(a.EmployeeId1, a.EmployeeId2),
            a => a
        );

        var result = new List<ColleaguePairDto>();

        foreach (var (id1, id2) in pairs)
        {
            if (!employees.TryGetValue(id1, out var emp1) || !employees.TryGetValue(id2, out var emp2))
                continue;

            affinityLookup.TryGetValue(NormalizePairKey(id1, id2), out var affinity);

            result.Add(new ColleaguePairDto
            {
                Employee1Id = emp1.Id,
                Employee1Name = $"{emp1.FirstName} {emp1.LastName}",
                Employee2Id = emp2.Id,
                Employee2Name = $"{emp2.FirstName} {emp2.LastName}",
                HasAffinityDefined = affinity != null,
                Score = affinity?.Score,
                Status = affinity?.Status.ToString() ?? AffinityStatus.Undefined.ToString()
            });
        }

        return result;
    }

    private static string NormalizePairKey(Guid id1, Guid id2)
    {
        // Normalizziamo l'ordine per garantire che (A,B) e (B,A) producano la stessa chiave
        return id1.CompareTo(id2) < 0 ? $"{id1}_{id2}" : $"{id2}_{id1}";
    }

    public async Task<ColleaguePairDto> SetAffinityAsync(SetAffinityDto dto)
    {
        var existing = await _affinityRepository.GetByEmployeePairAsync(dto.EmployeeId1, dto.EmployeeId2);

        // Deriviamo lo status dal punteggio — non lo chiediamo esplicitamente all'HR
        // per evitare inconsistenze tra i due valori (es. score alto con status Negative)
        var status = DeriveStatus(dto.Score);

        if (existing != null)
        {
            existing.Score = dto.Score;
            existing.Status = status;
            await _affinityRepository.UpdateAsync(existing);
        }
        else
        {
            existing = new EmployeeAffinity
            {
                Id = Guid.NewGuid(),
                EmployeeId1 = dto.EmployeeId1,
                EmployeeId2 = dto.EmployeeId2,
                Score = dto.Score,
                Status = status
            };
            await _affinityRepository.AddAsync(existing);
        }

        var employee1 = await _employeeRepository.GetByIdAsync(dto.EmployeeId1);
        var employee2 = await _employeeRepository.GetByIdAsync(dto.EmployeeId2);

        return new ColleaguePairDto
        {
            Employee1Id = dto.EmployeeId1,
            Employee1Name = $"{employee1!.FirstName} {employee1.LastName}",
            Employee2Id = dto.EmployeeId2,
            Employee2Name = $"{employee2!.FirstName} {employee2.LastName}",
            HasAffinityDefined = true,
            Score = existing.Score,
            Status = existing.Status.ToString()
        };
    }

    private static AffinityStatus DeriveStatus(int score)
    {
        if (score >= 4) return AffinityStatus.Positive;
        if (score <= 2) return AffinityStatus.Negative;
        return AffinityStatus.Neutral;
    }
}