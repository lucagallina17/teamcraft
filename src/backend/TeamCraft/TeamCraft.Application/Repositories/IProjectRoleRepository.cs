using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface IProjectRoleRepository : IGenericRepository<ProjectRole>
    {
        Task<ProjectRole?> GetByNameAsync(string name);
    }
}
