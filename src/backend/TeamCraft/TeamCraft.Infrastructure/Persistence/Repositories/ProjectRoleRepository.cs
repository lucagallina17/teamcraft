using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class ProjectRoleRepository : GenericRepository<ProjectRole>, IProjectRoleRepository
{
    public ProjectRoleRepository(AppDbContext context) : base(context) { }

    public async Task<ProjectRole?> GetByNameAsync(string name)
    {
        return await _context.ProjectRoles
            .FirstOrDefaultAsync(r => r.Name == name);
    }
}