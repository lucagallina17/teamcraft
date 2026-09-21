namespace TeamCraft.Domain.Entities;
public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email{ get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public UserAccount? UserAccount { get; set; }
    public ICollection<EmployeeCompetencyAssessment> CompetencyAssessments { get; set;} = new List<EmployeeCompetencyAssessment>();
    public ICollection<EmployeeAffinity> Affinities { get; set; } = new List<EmployeeAffinity>();
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();


}
