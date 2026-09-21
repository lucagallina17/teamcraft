namespace TeamCraft.Domain.Entities;

using TeamCraft.Domain.Enums;

public class EmployeeAffinity
{
    public Guid Id { get; set; }
    public Guid EmployeeId1 { get; set; }
    public Guid EmployeeId2 { get; set; }
    public int Score { get; set; }
    public AffinityStatus Status { get; set; }

    public Employee Employee1 { get; set; } = null!;
    public Employee Employee2 { get; set; } = null!;
}