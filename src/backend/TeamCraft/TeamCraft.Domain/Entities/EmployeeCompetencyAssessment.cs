namespace TeamCraft.Domain.Entities;

using TeamCraft.Domain.Enums;

public class EmployeeCompetencyAssessment
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid CompetencyId { get; set; }
    public int Level { get; set; }
    public AssessmentSource Source { get; set; }
    public DateTime Date { get; set; }

    public Employee Employee { get; set; } = null!;
    public Competency Competency { get; set; } = null!;
}