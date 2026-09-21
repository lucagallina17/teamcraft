    using Moq;
using MediatR;
using MassTransit;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Events;
using TeamCraft.Application.Commands;

namespace TeamCraft.Tests
{
    public class ActivateTeamCommandHandlerTests
    {
        [Fact]
        public async Task Handle_TeamWithMembers_ActivatesTeamAndReturnsDto()
        {
            // ============ ARRANGE ============

            // 1. Crea un TeamAggregate con almeno un membro (come hai già fatto per gli altri test)
            var team = new TeamAggregate(Guid.NewGuid());
            team.AddMember(Guid.NewGuid(), Guid.NewGuid());

            // 2. Crea i tre mock necessari
            var mockRepository = new Mock<ITeamAggregateRepository>();
            var mockMediator = new Mock<IMediator>();
            var mockPublishEndpoint = new Mock<IPublishEndpoint>();

            // 3. Configura il mock del repository per restituire il team quando viene chiamato GetByIdAsync
            //    (usa la sintassi che abbiamo già visto insieme)
            mockRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(team);


            // 4. Crea l'Handler passandogli i tre mock (non i mock "veri", ma il loro .Object — vedi nota sotto)
            var handler = new ActivateTeamCommandHandler(mockRepository.Object, mockMediator.Object, mockPublishEndpoint.Object);


            // ============ ACT ============

            // 5. Crea un ActivateTeamCommand con un TeamId qualsiasi, e chiama Handle()
            var command = new ActivateTeamCommand(Guid.NewGuid());
            var result = await handler.Handle(command, CancellationToken.None); 

            // ============ ASSERT ============

            // 6. Verifica che il risultato non sia null, e magari che ProjectId corrisponda
            Assert.NotNull(result);
            Assert.Equal(team.ProjectId, result.ProjectId);
        }
    }
}