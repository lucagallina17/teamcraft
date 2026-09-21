namespace TeamCraft.Domain.Entities
{
    public class ProjectRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<ProjectRoleRequirement> ProjectRoleRequirements { get; set; } = new List<ProjectRoleRequirement>();
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    }
}
