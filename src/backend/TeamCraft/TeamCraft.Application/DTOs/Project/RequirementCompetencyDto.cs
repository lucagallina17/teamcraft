using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.DTOs.Project
{
    public class RequirementCompetencyDto
    {
        public Guid Id { get; set; } // aggiunto — stesso errore già tracciato per EmployeeCompetencyDto
        public Guid CompetencyId { get; set; } // aggiunto — serve per precompilare il form di modifica
        public string CompetencyName { get; set; } = string.Empty;
        public int MinimumLevel { get; set; }
        public float Weight { get; set; }
        public RequirementType RequirementType { get; set; }
    }
}
