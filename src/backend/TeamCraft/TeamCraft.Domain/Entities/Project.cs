using TeamCraft.Domain.Enums;

namespace TeamCraft.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CreatedBy { get; set; }

        public ICollection<ProjectRoleRequirement> ProjectRoleRequirements { get; set;} = new List<ProjectRoleRequirement>();
        public ICollection<TeamAggregate> Teams { get; set;} = new List<TeamAggregate>();
    }
}
