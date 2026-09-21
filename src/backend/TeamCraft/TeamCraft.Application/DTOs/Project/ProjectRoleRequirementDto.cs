
namespace TeamCraft.Application.DTOs.Project
{
    public class ProjectRoleRequirementDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public List<RequirementCompetencyDto> Competencies { get; set; } = new();
    }
}
