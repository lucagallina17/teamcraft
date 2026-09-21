namespace TeamCraft.Application.DTOs.Employee;

public class SetAffinityDto
{
    public Guid EmployeeId1 { get; set; }
    public Guid EmployeeId2 { get; set; }
    public int Score { get; set; }
}