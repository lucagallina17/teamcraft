using TeamCraft.Domain.Enums;
using TeamCraft.Domain.Events;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Domain.Entities
{
    public class TeamAggregate
    {
        public Guid Id { get; private set; }
        public Guid ProjectId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public float Score { get; private set; }
        public TeamStatus Status { get; private set; }
        public Project Project { get; private set; } = null!;

        private readonly ICollection<TeamMember> _teamMembers = new List<TeamMember>();
        public IReadOnlyList<TeamMember> TeamMembers => _teamMembers.ToList();

        private readonly List<object> _events = new();
        public IReadOnlyList<object> DomainEvents => _events.ToList();

        public TeamAggregate(Guid projectId) { 
            Id = Guid.NewGuid();
            Status = TeamStatus.Proposed;
            Score = 0;
            ProjectId = projectId;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddMember(Guid employeeId, Guid projectRoleId)
        {
            if(Status != TeamStatus.Proposed)
            {
                throw new DomainRuleViolationException($"Non è più possibile aggiungere membri al team.");
            }

            if (_teamMembers.Any(tm => tm.EmployeeId == employeeId))
            {
                throw new DomainRuleViolationException($"Membro del team già presente.");
            }

            var teamMember = new TeamMember(Id, employeeId, projectRoleId);
            _teamMembers.Add(teamMember);
        }

        public void Activate()
        {
            if(_teamMembers.Count == 0)
            {
                throw new DomainRuleViolationException($"Il team è vuoto.");
            }
            Status = TeamStatus.Active;
            _events.Add(new TeamActivatedEvent(Id, ProjectId, DateTime.UtcNow));
        }

        public void Close()
        {
            if(Status != TeamStatus.Active)
            {
                throw new DomainRuleViolationException($"Il team non può essere chiuso.");
            }
            Status = TeamStatus.Closed;
        }

        public void RemoveMember(Guid employeeId)
        {
            if(Status == TeamStatus.Closed)
            {
                throw new DomainRuleViolationException($"Il team è chiuso.");
            }
            var teamMember = _teamMembers.FirstOrDefault(tm => tm.EmployeeId == employeeId ) ?? throw new DomainRuleViolationException($"Nessun membro trovato.");
            _teamMembers.Remove(teamMember);
        }

        public void SetScore(float score)
        {
            if(Status != TeamStatus.Proposed)
            {
                throw new DomainRuleViolationException($"Per poter aggiungere lo score il team deve essere nello stato Proposed");
            }
            Score = score;
        }

    }
}
