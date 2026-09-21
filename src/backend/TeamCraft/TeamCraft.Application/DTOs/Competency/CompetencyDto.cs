using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.DTOs.Competency
{
    public class CompetencyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CompetencyType Type { get; set; }
    }
}
