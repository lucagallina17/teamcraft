using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Enums;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class TeamAggregateRepository : ITeamAggregateRepository
{
    private readonly AppDbContext _context;
    public TeamAggregateRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<TeamAggregate?> GetByIdAsync(Guid id)
    {
        return await _context.Set<TeamAggregate>()
            .Include(t => t.Project)
            .Include(t => t.TeamMembers)
                .ThenInclude(tm => tm.Employee)
            .Include(t => t.TeamMembers)
                .ThenInclude(tm => tm.ProjectRole)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(TeamAggregate team)
    {
        await _context.Set<TeamAggregate>().AddAsync(team);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TeamAggregate team)
    {
        _context.Set<TeamAggregate>().Update(team);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var team = await GetByIdAsync(id);
        if(team != null)
        {
            _context.Set<TeamAggregate>().Remove(team);
            await _context.SaveChangesAsync();
        }
    }
}