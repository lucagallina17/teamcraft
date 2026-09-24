using TeamCraft.Application.Commands;
using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.DTOs.Team
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public float Score { get; set; }
        public TeamStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
        public TeamReviewDto? TeamReview { get; set; }
    }
}
