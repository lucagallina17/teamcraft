using TeamCraft.Domain.Entities; // o dove vive TeamAggregate — verifica il namespace esatto
using TeamCraft.Domain.Enums;
using TeamCraft.Domain.Events;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Tests
{
    public class TeamAggregateTests
    {
        [Fact]
        public void Activate_WithMembers_SetsStatusToActive()
        {
            // Arrange — crea un TeamAggregate e aggiungi almeno un membro
            var team = new TeamAggregate(Guid.NewGuid());
            team.AddMember(Guid.NewGuid(), Guid.NewGuid());

            // Act — chiama Activate()
            team.Activate();

            // Assert — verifica che Status sia diventato Active
            Assert.Equal(TeamStatus.Active, team.Status);
        }

        [Fact]
        public void Activate_WithoutMembers_ThrowsDomainRuleViolationException()
        {
            // Arrange
            var team = new TeamAggregate(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainRuleViolationException>(() => team.Activate());
        }

        [Fact]
        public void Activate_WithMembers_CheckTeamActivatedEvents()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var team = new TeamAggregate(projectId);
            team.AddMember(Guid.NewGuid(), Guid.NewGuid());

            // Act
            team.Activate();

            // Assert
            var domainEvent = Assert.Single(team.DomainEvents);
            var acivatedEvent = Assert.IsType<TeamActivatedEvent>(domainEvent);
            Assert.Equal(projectId, acivatedEvent.ProjectId);
        }

        [Fact]
        public void AddMember_DuplicateEmployee_ThrowsDomainRuleViolationException()
        {
            // Arrange
            var team = new TeamAggregate(Guid.NewGuid());
            var employeeId = Guid.NewGuid();
            var projectRoleId = Guid.NewGuid();
            team.AddMember(employeeId, projectRoleId);

            // Act & Assert
            Assert.Throws<DomainRuleViolationException>(() => team.AddMember(employeeId, projectRoleId));
        }
    }
}