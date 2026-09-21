using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Exceptions;

namespace Tests
{
    public class TeamReviewAggregateTests
    {
        [Fact]
        public void Add_WithValidData_CreateReview()
        {
            // Arrange
            var teamId = Guid.NewGuid();

            // Act
            var teamReview = new TeamReviewAggregate(4, "Descrizione", teamId);

            // Assert
            Assert.Equal(4, teamReview.Score);
            Assert.Equal("Descrizione", teamReview.Description);
            Assert.Equal(teamId, teamReview.TeamId);
        }

        [Fact]
        public void Constructor_WithInvalidScore_ThrowsDomainRuleViolationException()
        {
            // Arrange
            var teamId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainRuleViolationException>(() => new TeamReviewAggregate(8, "Descrizione", teamId));
        }

        [Fact]
        public void UpdateScore_WithInvalidScore_ThrowsDomainRuleViolationException()
        {
            // Arrange
            var teamId = Guid.NewGuid();

            // Act
            var teamReview = new TeamReviewAggregate(4, "Descrizione", teamId);

            // Assert
            Assert.Throws<DomainRuleViolationException>(() => teamReview.UpdateScore(8));
        }
    }
}
