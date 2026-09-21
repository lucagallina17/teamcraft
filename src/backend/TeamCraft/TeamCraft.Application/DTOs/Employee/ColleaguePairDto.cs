namespace TeamCraft.Application.DTOs.Employee;

public class ColleaguePairDto
{
    public Guid Employee1Id { get; set; }
    public string Employee1Name { get; set; } = string.Empty;
    public Guid Employee2Id { get; set; }
    public string Employee2Name { get; set; } = string.Empty;
    public bool HasAffinityDefined { get; set; }
    public int? Score { get; set; }
    public string Status { get; set; } = string.Empty;
}