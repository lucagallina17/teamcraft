using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class TeamReviewAggregateRepository : GenericRepository<TeamReviewAggregate>, ITeamReviewAggregateRepository
{
    public TeamReviewAggregateRepository(AppDbContext context) : base(context) { }

    public async Task<TeamReviewAggregate?> GetByTeamIdAsync(Guid teamId)
    {
        return await _context.TeamReviewAggregate
            .FirstOrDefaultAsync(t => t.TeamId == teamId);
    }
}
