using POPs.Shared.Models;

namespace POPs.Api.Data;

/// <summary>
/// Заполняет БД демо-данными при первом запуске.
/// </summary>
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Administrators.Any())
        {
            return;
        }

        var admin = new Administrator
        {
            Id = Guid.NewGuid(),
            FullName = "Администратор Системы",
            Login = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin")
        };
        db.Administrators.Add(admin);

        var course1 = new Course { Number = 1, Name = "1 курс", StartYear = 2025, EndYear = 2026 };
        var course2 = new Course { Number = 2, Name = "2 курс", StartYear = 2024, EndYear = 2025 };
        db.Courses.AddRange(course1, course2);
        db.SaveChanges();

        var groupA = new Group { Name = "ИСП-101", Curator = "Иванова А.П.", CourseId = course1.Id };
        var groupB = new Group { Name = "ИСП-201", Curator = "Петров С.И.", CourseId = course2.Id };
        db.Groups.AddRange(groupA, groupB);
        db.SaveChanges();

        db.Students.AddRange(
            new Student
            {
                GroupId = groupA.Id,
                FullName = "Стремилов Лев",
                BirthDate = new DateOnly(2004, 5, 12),
                Phone = "+7-900-111-22-33",
                Email = "stremilov@example.com",
                Address = "г. Москва",
                Status = "Обучается",
                PreviousEducation = "Школа №1",
                GraduationYear = 2022,
                Login = "student",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("student")
            },
            new Student
            {
                GroupId = groupA.Id,
                FullName = "Сидорова Мария",
                BirthDate = new DateOnly(2005, 3, 21),
                Phone = "+7-900-222-33-44",
                Email = "sidorova@example.com",
                Address = "г. Москва",
                Status = "Обучается",
                PreviousEducation = "Лицей №5",
                GraduationYear = 2023,
                Login = "sidorova",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("student")
            },
            new Student
            {
                GroupId = groupB.Id,
                FullName = "Козлов Иван",
                BirthDate = new DateOnly(2003, 11, 8),
                Phone = "+7-900-333-44-55",
                Email = "kozlov@example.com",
                Address = "г. Москва",
                Status = "Обучается",
                PreviousEducation = "Колледж",
                GraduationYear = 2021,
                Login = "kozlov",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("student")
            }
        );

        db.SaveChanges();
    }
}
