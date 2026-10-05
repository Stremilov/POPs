using Microsoft.EntityFrameworkCore;
using POPs.Api.Data;
using POPs.Shared.Dtos;

namespace POPs.Api.Services;

/// <summary>
/// Сервис отчётности (по диаграмме активности).
/// </summary>
public class ReportService
{
    private readonly AppDbContext _db;

    public ReportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ContingentItemDto>> GetContingentAsync()
    {
        var students = await _db.Students
            .Include(s => s.Group)!
            .ThenInclude(g => g!.Course)
            .OrderBy(s => s.FullName)
            .ToListAsync();

        return students.Select(s => new ContingentItemDto
        {
            StudentId = s.Id,
            FullName = s.FullName,
            Status = s.Status,
            GroupName = s.Group?.Name ?? "—",
            CourseName = s.Group?.Course?.Name ?? "—"
        }).ToList();
    }

    public async Task<int> CountByStatusAsync(string status)
    {
        return await _db.Students.CountAsync(s => s.Status == status);
    }

    public async Task<int> CountByGroupAsync(int groupId)
    {
        return await _db.Students.CountAsync(s => s.GroupId == groupId);
    }

    public async Task<int> CountByCourseAsync(int courseId)
    {
        return await _db.Students
            .Include(s => s.Group)
            .CountAsync(s => s.Group != null && s.Group.CourseId == courseId);
    }

    /// <summary>
    /// Отчёт по группе — сценарий из диаграммы активности:
    /// запросить данные группы → получить студентов → если есть — статистика, иначе пустой отчёт.
    /// </summary>
    public async Task<GroupReportDto?> GetGroupStatisticsAsync(int groupId)
    {
        var group = await _db.Groups
            .Include(g => g.Course)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            return null;
        }

        var students = group.Students.OrderBy(s => s.FullName).ToList();

        // Ветка «Студенты есть?» из диаграммы активности
        if (students.Count == 0)
        {
            return new GroupReportDto
            {
                GroupId = group.Id,
                GroupName = group.Name,
                Curator = group.Curator,
                CourseName = group.Course?.Name ?? "—",
                IsEmpty = true,
                TotalStudents = 0,
                ByStatus = new Dictionary<string, int>(),
                Students = new List<StudentDto>()
            };
        }

        var byStatus = students
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new GroupReportDto
        {
            GroupId = group.Id,
            GroupName = group.Name,
            Curator = group.Curator,
            CourseName = group.Course?.Name ?? "—",
            IsEmpty = false,
            TotalStudents = students.Count,
            ByStatus = byStatus,
            Students = students.Select(s => new StudentDto
            {
                Id = s.Id,
                GroupId = s.GroupId,
                GroupName = group.Name,
                FullName = s.FullName,
                BirthDate = s.BirthDate,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                Status = s.Status,
                PreviousEducation = s.PreviousEducation,
                GraduationYear = s.GraduationYear,
                Login = s.Login
            }).ToList()
        };
    }

    public async Task<CourseStatisticsDto?> GetCourseStatisticsAsync(int courseId)
    {
        var course = await _db.Courses
            .Include(c => c.Groups)
            .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            return null;
        }

        var students = course.Groups.SelectMany(g => g.Students).ToList();
        var byStatus = students
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new CourseStatisticsDto
        {
            CourseId = course.Id,
            CourseName = course.Name,
            GroupsCount = course.Groups.Count,
            StudentsCount = students.Count,
            ByStatus = byStatus
        };
    }

    public async Task<OverviewStatisticsDto> GetOverviewAsync()
    {
        var students = await _db.Students.ToListAsync();
        var byStatus = students
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new OverviewStatisticsDto
        {
            TotalStudents = students.Count,
            TotalGroups = await _db.Groups.CountAsync(),
            TotalCourses = await _db.Courses.CountAsync(),
            ByStatus = byStatus
        };
    }
}
