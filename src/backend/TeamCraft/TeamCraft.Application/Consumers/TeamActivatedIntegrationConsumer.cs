using MassTransit;
using TeamCraft.Application.IntegrationEvents; // o dove hai messo l'evento

namespace TeamCraft.Application.Consumers  // o Handlers, a tua scelta
{
    public class TeamActivatedIntegrationConsumer : IConsumer<TeamActivatedIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<TeamActivatedIntegrationEvent> context)
        {
            Console.WriteLine(context.Message.ProjectId);
        }
    }
}