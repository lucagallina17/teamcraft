using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Interfaces.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class EmployeeAffinityRepository : GenericRepository<EmployeeAffinity>, IEmployeeAffinityRepository
{
    public EmployeeAffinityRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<EmployeeAffinity>> GetForEmployeesAsync(IEnumerable<Guid> employeeIds)
    {
        var ids = employeeIds.ToHashSet();

        return await _context.EmployeeAffinities
            .Where(a => ids.Contains(a.EmployeeId1) && ids.Contains(a.EmployeeId2))
            .ToListAsync();
    }

    public async Task<EmployeeAffinity?> GetByEmployeePairAsync(Guid employeeId1, Guid employeeId2)
    {
        // La coppia (A, B) è equivalente a (B, A) — l'affinità non ha una direzione
        return await _context.EmployeeAffinities
            .Include(a => a.Employee1)
            .Include(a => a.Employee2)
            .FirstOrDefaultAsync(a =>
                (a.EmployeeId1 == employeeId1 && a.EmployeeId2 == employeeId2) ||
                (a.EmployeeId1 == employeeId2 && a.EmployeeId2 == employeeId1));
    }

    public async Task<IEnumerable<(Guid EmployeeId1, Guid EmployeeId2)>> GetColleaguePairsAsync()
    {
        var teamMemberships = await _context.Set<TeamMember>()
            .Select(tm => new { tm.TeamId, tm.EmployeeId })
            .ToListAsync();

        var pairs = new HashSet<(Guid, Guid)>();

        var groupedByTeam = teamMemberships.GroupBy(tm => tm.TeamId);

        foreach (var team in groupedByTeam)
        {
            var employeeIds = team.Select(tm => tm.EmployeeId).Distinct().ToList();

            for (int i = 0; i < employeeIds.Count; i++)
            {
                for (int j = i + 1; j < employeeIds.Count; j++)
                {
                    // Normalizziamo l'ordine della coppia per evitare duplicati (A,B) e (B,A)
                    var pair = employeeIds[i].CompareTo(employeeIds[j]) < 0
                        ? (employeeIds[i], employeeIds[j])
                        : (employeeIds[j], employeeIds[i]);

                    pairs.Add(pair);
                }
            }
        }

        return pairs;
    }
}