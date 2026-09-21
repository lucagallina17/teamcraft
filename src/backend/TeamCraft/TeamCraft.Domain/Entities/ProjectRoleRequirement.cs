namespace TeamCraft.Domain.Entities
{
    public class ProjectRoleRequirement
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectRoleId { get; set; }
        public int Quantity { get; set; }

        public Project Project { get; set; } = null!;
        public ProjectRole ProjectRole { get; set; } = null!;
        public ICollection<ProjectRoleRequirementCompetency> RequirementCompetencies { get; set; } = new List<ProjectRoleRequirementCompetency>();
    }
}
