using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface ITeamAggregateRepository
    {
        Task<TeamAggregate?> GetByIdAsync(Guid id);
        Task AddAsync(TeamAggregate team);
        Task UpdateAsync(TeamAggregate team);
        Task DeleteAsync(Guid id);
    }
}
