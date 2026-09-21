namespace TeamCraft.Domain.Entities
{
    public class TeamMember
    {
        public Guid Id { get; private set; }  // dato scalare — il costruttore lo può impostare
        public Guid TeamId { get; private set; }  // dato scalare — il costruttore lo può impostare
        public Guid EmployeeId { get; private set; }  // dato scalare — il costruttore lo può impostare
        public Guid ProjectRoleId { get; private set; }  // dato scalare — il costruttore lo può impostare

        public TeamAggregate Team { get; private set; } = null!; // navigation property
        public Employee Employee { get; private set; } = null!; // navigation property
        public ProjectRole ProjectRole { get; private set; } = null!; // navigation property

        internal TeamMember(Guid teamId, Guid employeeId, Guid projectRoleId) {
            Id = Guid.NewGuid();
            TeamId = teamId;
            EmployeeId = employeeId;
            ProjectRoleId = projectRoleId;
        }
    }
}
