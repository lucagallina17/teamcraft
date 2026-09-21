using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Infrastructure.Persistence;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class ProjectRepository : GenericRepository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public async Task<Project?> GetByIdWithRequirementsAsync(Guid id)
    {
        return await _context.Projects
            .Include(p => p.ProjectRoleRequirements)
                .ThenInclude(r => r.ProjectRole)
            .Include(p => p.ProjectRoleRequirements)
                .ThenInclude(r => r.RequirementCompetencies)
                    .ThenInclude(c => c.Competency)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Project>> GetAllWithRequirementsAsync()
    {
        return await _context.Projects
            .Include(p => p.ProjectRoleRequirements)
                .ThenInclude(r => r.ProjectRole)
            .ToListAsync();
    }

    public async Task<ProjectRoleRequirement> AddRequirementAsync(ProjectRoleRequirement requirement)
    {
        _context.Set<ProjectRoleRequirement>().Add(requirement);
        await _context.SaveChangesAsync();
        return requirement;
    }

    public async Task<ProjectRoleRequirement?> GetRequirementByIdAsync(Guid requirementId)
    {
        return await _context.Set<ProjectRoleRequirement>()
            .Include(r => r.ProjectRole)
            .FirstOrDefaultAsync(r => r.Id == requirementId);
    }

    public async Task AddRequirementCompetencyAsync(ProjectRoleRequirementCompetency competency)
    {
        _context.Set<ProjectRoleRequirementCompetency>().Add(competency);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRequirementAsync(ProjectRoleRequirement requirement)
    {
        _context.Set<ProjectRoleRequirement>().Remove(requirement);
        await _context.SaveChangesAsync();
    }

    public async Task<ProjectRoleRequirementCompetency?> GetRequirementCompetencyByIdAsync(Guid id)
    {
        return await _context.Set<ProjectRoleRequirementCompetency>()
            .Include(c => c.Competency)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task UpdateRequirementCompetencyAsync(ProjectRoleRequirementCompetency competency)
    {
        _context.Set<ProjectRoleRequirementCompetency>().Update(competency);
        await _context.SaveChangesAsync();
    }
}