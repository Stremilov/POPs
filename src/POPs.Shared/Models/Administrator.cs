namespace POPs.Shared.Models;

/// <summary>
/// Администратор системы.
/// </summary>
public class Administrator
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}
