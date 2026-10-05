using Microsoft.EntityFrameworkCore;
using POPs.Api.Data;
using POPs.Shared.Dtos;
using POPs.Shared.Models;

namespace POPs.Api.Services;

/// <summary>
/// Сервис управления курсами.
/// </summary>
public class CourseService
{
    private readonly AppDbContext _db;

    public CourseService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        var courses = await _db.Courses
            .Include(c => c.Groups)
            .OrderBy(c => c.Number)
            .ToListAsync();

        return courses.Select(ToDto).ToList();
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _db.Courses.Include(c => c.Groups).FirstOrDefaultAsync(c => c.Id == id);
        return course == null ? null : ToDto(course);
    }

    public async Task<CourseDto> CreateCourseAsync(CourseCreateRequest request)
    {
        var course = new Course
        {
            Number = request.Number,
            Name = request.Name,
            StartYear = request.StartYear,
            EndYear = request.EndYear
        };

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task<CourseDto?> EditCourseAsync(CourseUpdateRequest request)
    {
        var course = await _db.Courses.Include(c => c.Groups).FirstOrDefaultAsync(c => c.Id == request.Id);
        if (course == null)
        {
            return null;
        }

        course.Number = request.Number;
        course.Name = request.Name;
        course.StartYear = request.StartYear;
        course.EndYear = request.EndYear;

        await _db.SaveChangesAsync();
        return ToDto(course);
    }

    public async Task AddGroupToCourseAsync(int groupId, int courseId)
    {
        var group = await _db.Groups.FindAsync(groupId)
            ?? throw new InvalidOperationException("Группа не найдена.");

        var courseExists = await _db.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        group.CourseId = courseId;
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Перевод студентов на следующий курс: создаёт/находит курс N+1
    /// и переносит все группы текущего курса туда.
    /// </summary>
    public async Task<string> PromoteStudentsAsync(int courseId)
    {
        var course = await _db.Courses
            .Include(c => c.Groups)
            .ThenInclude(g => g.Students)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        var nextNumber = course.Number + 1;
        var nextCourse = await _db.Courses.FirstOrDefaultAsync(c => c.Number == nextNumber);

        if (nextCourse == null)
        {
            nextCourse = new Course
            {
                Number = nextNumber,
                Name = $"{nextNumber} курс",
                StartYear = course.StartYear + 1,
                EndYear = course.EndYear + 1
            };
            _db.Courses.Add(nextCourse);
            await _db.SaveChangesAsync();
        }

        var movedGroups = 0;
        var movedStudents = 0;

        foreach (var group in course.Groups)
        {
            group.CourseId = nextCourse.Id;
            movedGroups++;
            movedStudents += group.Students.Count(s => s.Status == "Обучается");
        }

        await _db.SaveChangesAsync();
        return $"Переведено групп: {movedGroups}, студентов (обучаются): {movedStudents} на курс «{nextCourse.Name}».";
    }

    private static CourseDto ToDto(Course c) => new()
    {
        Id = c.Id,
        Number = c.Number,
        Name = c.Name,
        StartYear = c.StartYear,
        EndYear = c.EndYear,
        GroupsCount = c.Groups?.Count ?? 0
    };
}
