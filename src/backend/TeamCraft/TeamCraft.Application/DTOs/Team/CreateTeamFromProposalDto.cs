namespace TeamCraft.Application.DTOs.Team;

public class CreateTeamFromProposalDto
{
    public List<CreateTeamMemberDto> Members { get; set; } = new();
}

public class CreateTeamMemberDto
{
    public Guid EmployeeId { get; set; }
    public Guid ProjectRoleRequirementId { get; set; }
    public Guid ProjectRoleId { get; set; }
}