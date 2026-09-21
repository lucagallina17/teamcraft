namespace TeamCraft.Application.DTOs.Team;

public class TeamProposalDto
{
    public Guid ProposalId { get; set; }
    public double TotalScore { get; set; }
    public bool IsComplete { get; set; } // false se non c'erano candidati eleggibili per tutti gli slot
    public List<TeamProposalMemberDto> Members { get; set; } = new();
}

public class TeamProposalMemberDto
{
    public Guid EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid ProjectRoleRequirementId { get; set; }
    public Guid ProjectRoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public double CompetencyScore { get; set; }
}