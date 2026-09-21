using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.DTOs.Employee
{
    public class EmployeeCompetencyDto
    {
        public Guid Id { get; set; }
        public Guid CompetencyId { get; set; }
        public string CompetencyName { get; set; } = string.Empty;
        public CompetencyType CompetencyType { get; set; }
        public int Level { get; set; }
        public string Source { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
