using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.DTOs.Team;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Enums;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class TeamReadRepository : ITeamReadRepository
{
    private readonly AppDbContext _context;
    public TeamReadRepository(AppDbContext context) { 
        _context = context;
    }

    public async Task<TeamDto?> GetByIdWithMembersAsync(Guid id)
    {
        return await _context.Set<TeamAggregate>()
            .Where(x => x.Id == id)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                Score = t.Score,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                Members = t.TeamMembers.Select(tm => new TeamMemberDto
                {
                    Id = tm.Id,
                    EmployeeId = tm.EmployeeId,
                    FullName = tm.Employee.FirstName + " " + tm.Employee.LastName,
                    RoleName = tm.ProjectRole.Name
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TeamDto>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Set<TeamAggregate>()
            .Where(x => x.ProjectId == projectId)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                Score = t.Score,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                Members = t.TeamMembers.Select(tm => new TeamMemberDto
                {
                    Id = tm.Id,
                    EmployeeId = tm.EmployeeId,
                    FullName = tm.Employee.FirstName + " " + tm.Employee.LastName,
                    RoleName = tm.ProjectRole.Name
                }).ToList()
            }).ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetEmployeeIdsInActiveTeamsAsync()
    {
        return await _context.Set<TeamMember>()
            .Where(tm => tm.Team.Status == TeamStatus.Active)
            .Select(tm => tm.EmployeeId)
            .Distinct()
            .ToListAsync();
    }
}