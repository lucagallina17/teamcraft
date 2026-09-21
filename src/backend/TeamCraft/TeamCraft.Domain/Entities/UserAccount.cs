namespace TeamCraft.Domain.Entities;

public class UserAccount
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public Employee Employee { get; set; } = null!; // Navigation property per accedere al oggetto Employee che è stato referenziato tramite la FK EmployeeId
}
