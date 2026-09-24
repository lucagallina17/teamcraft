using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories;

public interface ITeamReviewAggregateRepository
{
    Task<TeamReviewAggregate?> GetByIdAsync(Guid id);
    Task<TeamReviewAggregate?> GetByTeamIdAsync(Guid teamId);
    Task AddAsync(TeamReviewAggregate teamReview);
    Task UpdateAsync(TeamReviewAggregate teamReview);
}
