using System.ComponentModel.DataAnnotations;

namespace TeamCraft.Application.DTOs.Project;

public class AddRequirementDto
{
    public Guid ProjectRoleId { get; set; }
    public int Quantity { get; set; }
}

public class AddRequirementCompetencyDto
{
    public Guid CompetencyId { get; set; }

    [Range(1, 5)]
    public int MinimumLevel { get; set; }

    [Range(1, 5)]
    public float Weight { get; set; }   
    public string RequirementType { get; set; } = string.Empty;
}