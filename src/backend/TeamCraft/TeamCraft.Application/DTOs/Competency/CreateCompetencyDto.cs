using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.DTOs.Competency
{
    public class CreateCompetencyDto
    {
        public string Name { get; set; } = string.Empty;    
        public CompetencyType Type { get; set; }
    }
}
