namespace TeamCraft.Application.DTOs.Employee;

public class EmployeeTeamHistoryDto
{
    public Guid TeamId { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public string TeamStatus { get; set; } = string.Empty;
    public DateTime TeamCreatedAt { get; set; }
}