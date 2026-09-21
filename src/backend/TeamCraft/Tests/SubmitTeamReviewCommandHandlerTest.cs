using Moq;
using TeamCraft.Application.Commands;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Exceptions;

namespace Tests
{
    public class SubmitTeamReviewCommandHandlerTest
    {
        [Fact]
        public async Task Handle_TeamReview_InvalidStatus()
        {
            // Arrange
            var team = new TeamAggregate(Guid.NewGuid());

            var mockTeamAggregateRepository = new Mock<ITeamAggregateRepository>();
            var mockTeamReviewAggregateRepository = new Mock<ITeamReviewAggregateRepository>();

            mockTeamAggregateRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(team);

            var handler = new SubmitTeamReviewCommandHandler(mockTeamAggregateRepository.Object, mockTeamReviewAggregateRepository.Object);

            // Act
            var command = new SubmitTeamReview(Guid.NewGuid(), 2, "Descrizione");

            // Assert
            await Assert.ThrowsAsync<DomainRuleViolationException>(()=> handler.Handle(command));

            mockTeamReviewAggregateRepository.Verify(r => r.AddAsync(It.IsAny<TeamReviewAggregate>()), Times.Never);
        }
    }
}
