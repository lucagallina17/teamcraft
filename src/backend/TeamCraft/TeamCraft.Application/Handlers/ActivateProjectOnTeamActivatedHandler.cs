using MediatR;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Events;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Application.Handlers
{
    public class ActivateProjectOnTeamActivatedHandler : INotificationHandler<TeamActivatedEvent>
    {
        private readonly IProjectRepository _projectRepository;
        public ActivateProjectOnTeamActivatedHandler(IProjectRepository projectRepository) {
            _projectRepository = projectRepository;
        }
        public async Task Handle(TeamActivatedEvent notification, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetByIdAsync(notification.ProjectId) ?? throw new EntityNotFoundException($"Progetto con ID {notification.ProjectId} non trovato.");

            project.Status = Domain.Enums.ProjectStatus.Active;

            await _projectRepository.UpdateAsync(project);
        }
    }
}
