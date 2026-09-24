namespace TeamCraft.Application.DTOs.Team;

public class SubmitTeamReviewDto
{
    public int Score { get; set; }
    public string Description { get; set; } = string.Empty;
}