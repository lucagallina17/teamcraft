namespace TeamCraft.Application.DTOs.Employee
{
    public class EmployeeDetailDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<EmployeeCompetencyDto> Competencies { get; set; } = new();
        public List<EmployeeTeamHistoryDto> TeamHistory { get; set; } = new();
    }
}
