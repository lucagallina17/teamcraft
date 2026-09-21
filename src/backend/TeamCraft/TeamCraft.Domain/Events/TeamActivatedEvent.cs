using MediatR;

namespace TeamCraft.Domain.Events
{
    public record TeamActivatedEvent(Guid TeamId, Guid ProjectId, DateTime CreatedAt) : INotification;
}
