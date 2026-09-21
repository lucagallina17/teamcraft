namespace TeamCraft.Application.DTOs.Employee;

public class AddCompetencyAssessmentDto
{
    public Guid CompetencyId { get; set; }
    public int Level { get; set; }
    public string Source { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}