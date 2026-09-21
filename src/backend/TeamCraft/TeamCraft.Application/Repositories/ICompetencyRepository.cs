using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface ICompetencyRepository : IGenericRepository<Competency>
    {
        Task<Competency?> GetByNameAsync(string name);
    }
}
