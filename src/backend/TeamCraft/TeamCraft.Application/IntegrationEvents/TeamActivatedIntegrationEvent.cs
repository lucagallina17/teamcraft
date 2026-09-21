
namespace TeamCraft.Application.IntegrationEvents
{
    public record TeamActivatedIntegrationEvent(Guid ProjectId, Guid TeamId, DateTime CreatedAt);
}
