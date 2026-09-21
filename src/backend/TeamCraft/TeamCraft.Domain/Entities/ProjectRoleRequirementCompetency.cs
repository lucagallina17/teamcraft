using TeamCraft.Domain.Enums;

namespace TeamCraft.Domain.Entities
{
    public class ProjectRoleRequirementCompetency
    {
        public Guid Id { get; set; }
        public Guid ProjectRoleRequirementId { get; set; }
        public Guid CompetencyId { get; set; }
        public int MinimumLevel { get; set; }
        public float Weight { get; set; }
        public RequirementType RequirementType { get; set; }

        public ProjectRoleRequirement ProjectRoleRequirement { get; set; } = null!;
        public Competency Competency { get; set; } = null!;
    }
}
