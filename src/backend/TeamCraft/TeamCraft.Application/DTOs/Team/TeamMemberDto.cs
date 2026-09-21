namespace TeamCraft.Application.DTOs.Team
{
    public class TeamMemberDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
