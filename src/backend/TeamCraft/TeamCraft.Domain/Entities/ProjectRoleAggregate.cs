using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Domain.Entities
{
    public class ProjectRoleAggregate
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        private readonly ICollection<ProjectRoleRequirement> _projectRoleRequirements = new List<ProjectRoleRequirement>();
        public IReadOnlyCollection<ProjectRoleRequirement> ProjectRoleRequirements => _projectRoleRequirements.ToList();
        public IReadOnlyCollection<TeamMember> TeamMembers { get; } = new List<TeamMember>();

        public ProjectRoleAggregate(string name, string description) { 
            Id = Guid.NewGuid();
            Name = _checkNullOrEmptyAndLengthRoleName(name);
            Description = description;
        }

        public void Rename(string newName)
        {
            Name = _checkNullOrEmptyAndLengthRoleName(newName);
        }

        private static string _checkNullOrEmptyAndLengthRoleName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new DomainRuleViolationException("Il nome del ruolo non può essere vuoto");
            }
            if (newName.Length < 3)
            {
                throw new DomainRuleViolationException($"Il nome del ruolo deve avere almeno 3 caratteri");
            }
            return newName;
        }
    }
}
