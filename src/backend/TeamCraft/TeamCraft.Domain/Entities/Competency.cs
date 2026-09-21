namespace TeamCraft.Domain.Entities;

using TeamCraft.Domain.Enums;

    public class Competency
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CompetencyType Type { get; set; }

        public ICollection<EmployeeCompetencyAssessment> Assessments { get; set; } = new List<EmployeeCompetencyAssessment>();
        public ICollection<ProjectRoleRequirementCompetency> ProjectRoleRequirementCompetencies { get; set; } = new List<ProjectRoleRequirementCompetency>();
    }

