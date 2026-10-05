using Microsoft.EntityFrameworkCore;
using POPs.Api.Data;
using POPs.Shared.Dtos;

namespace POPs.Api.Services;

/// <summary>
/// Простая аутентификация администратора и студента.
/// </summary>
public class AuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var login = request.Login.Trim();

        var admin = await _db.Administrators.FirstOrDefaultAsync(a => a.Login == login);
        if (admin != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                return Fail("Неверный логин или пароль.");
            }

            return new LoginResponse
            {
                Success = true,
                Message = "Вход выполнен",
                Role = "Admin",
                FullName = admin.FullName,
                Login = admin.Login,
                AdminId = admin.Id
            };
        }

        var student = await _db.Students.FirstOrDefaultAsync(s => s.Login == login);
        if (student != null)
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
            {
                return Fail("Неверный логин или пароль.");
            }

            return new LoginResponse
            {
                Success = true,
                Message = "Вход выполнен",
                Role = "Student",
                FullName = student.FullName,
                Login = student.Login,
                StudentId = student.Id
            };
        }

        return Fail("Пользователь не найден.");
    }

    private static LoginResponse Fail(string message) => new()
    {
        Success = false,
        Message = message
    };
}
