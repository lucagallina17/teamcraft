using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Application.Commands
{
    public class TeamReviewDto{
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string Description { get; set; } = string.Empty;
    }
    public record SubmitTeamReview(Guid TeamId, int Score, string Description);
    public class SubmitTeamReviewCommandHandler
    {
        private readonly ITeamAggregateRepository _teamAggregateRepository;
        private readonly ITeamReviewAggregateRepository _teamReviewAggregateRepository;
        public SubmitTeamReviewCommandHandler(ITeamAggregateRepository teamAggregateRepository, ITeamReviewAggregateRepository teamReviewAggregateRepository) {
            _teamAggregateRepository = teamAggregateRepository;
            _teamReviewAggregateRepository = teamReviewAggregateRepository;
        }

        public async Task<TeamReviewDto> Handle(SubmitTeamReview command)
        {
            var team = await _teamAggregateRepository.GetByIdAsync(command.TeamId) ?? throw new EntityNotFoundException($"Non è stato trovato nessun team.");

            if(team.Status != Domain.Enums.TeamStatus.Closed)
            {
                throw new DomainRuleViolationException($"Il team deve essere chiuso per poterlo recensire");
            }

            var newTeamReview = new TeamReviewAggregate(command.Score, command.Description, command.TeamId);

            await _teamReviewAggregateRepository.AddAsync(newTeamReview);

            var teamReviewDto = new TeamReviewDto
            {
                Id = newTeamReview.Id,
                Score = newTeamReview.Score,
                Description = newTeamReview.Description,
            };
            return teamReviewDto;
        }
    }
}
