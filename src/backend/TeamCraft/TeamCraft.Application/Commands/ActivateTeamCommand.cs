using MassTransit;
using MediatR;
using TeamCraft.Application.IntegrationEvents;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Application.Commands
{
    public class TeamCommandResultDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }
    public record ActivateTeamCommand(Guid TeamId) : IRequest<TeamCommandResultDto>;
    public class ActivateTeamCommandHandler : IRequestHandler<ActivateTeamCommand, TeamCommandResultDto>
    {
        private readonly ITeamAggregateRepository _teamAggregateRepository;
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;

        public ActivateTeamCommandHandler(ITeamAggregateRepository teamAggregateRepository, IMediator mediator, IPublishEndpoint publishEndpoint)
        {
            _teamAggregateRepository = teamAggregateRepository;
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<TeamCommandResultDto> Handle(ActivateTeamCommand command, CancellationToken cancellationToken)
        {
            var team = await _teamAggregateRepository.GetByIdAsync(command.TeamId) ?? throw new DomainRuleViolationException($"Non è stato trovato nessun team.");

            team.Activate();

            await _teamAggregateRepository.UpdateAsync(team);

            foreach (var domainEvent in team.DomainEvents)
            {
                await _mediator.Publish(domainEvent);
            }

            await _publishEndpoint.Publish(new TeamActivatedIntegrationEvent(team.ProjectId, team.Id, DateTime.UtcNow));

            var teamDto =  new TeamCommandResultDto
            {
                Id = team.Id,
                ProjectId = team.ProjectId,
            };

            return teamDto;
        }
    }
}
