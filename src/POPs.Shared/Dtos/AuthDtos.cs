namespace POPs.Shared.Dtos;

public class LoginRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Admin | Student
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public int? StudentId { get; set; }
    public Guid? AdminId { get; set; }
}
