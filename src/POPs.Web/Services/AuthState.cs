using POPs.Shared.Dtos;

namespace POPs.Web.Services;

/// <summary>
/// Хранит текущего пользователя в памяти браузера (простая сессия).
/// </summary>
public class AuthState
{
    public bool IsAuthenticated => !string.IsNullOrEmpty(Role);
    public bool IsAdmin => Role == "Admin";
    public bool IsStudent => Role == "Student";

    public string Role { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Login { get; private set; } = string.Empty;
    public int? StudentId { get; private set; }
    public Guid? AdminId { get; private set; }

    public event Action? OnChange;

    public void SetUser(LoginResponse response)
    {
        Role = response.Role;
        FullName = response.FullName;
        Login = response.Login;
        StudentId = response.StudentId;
        AdminId = response.AdminId;
        OnChange?.Invoke();
    }

    public void Logout()
    {
        Role = string.Empty;
        FullName = string.Empty;
        Login = string.Empty;
        StudentId = null;
        AdminId = null;
        OnChange?.Invoke();
    }
}
