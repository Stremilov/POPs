using Microsoft.EntityFrameworkCore;
using POPs.Api.Data;
using POPs.Shared.Dtos;
using POPs.Shared.Models;

namespace POPs.Api.Services;

/// <summary>
/// Сервис управления группами.
/// </summary>
public class GroupService
{
    private readonly AppDbContext _db;

    public GroupService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<GroupDto>> GetAllAsync()
    {
        var groups = await _db.Groups
            .Include(g => g.Course)
            .Include(g => g.Students)
            .OrderBy(g => g.Name)
            .ToListAsync();

        return groups.Select(ToDto).ToList();
    }

    public async Task<GroupDto?> GetByIdAsync(int id)
    {
        var group = await _db.Groups
            .Include(g => g.Course)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == id);

        return group == null ? null : ToDto(group);
    }

    public async Task<GroupDto> CreateGroupAsync(GroupCreateRequest request)
    {
        var courseExists = await _db.Courses.AnyAsync(c => c.Id == request.CourseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        var group = new Group
        {
            Name = request.Name,
            Curator = request.Curator,
            CourseId = request.CourseId
        };

        _db.Groups.Add(group);
        await _db.SaveChangesAsync();

        await _db.Entry(group).Reference(g => g.Course).LoadAsync();
        return ToDto(group);
    }

    public async Task<GroupDto?> EditGroupAsync(GroupUpdateRequest request)
    {
        var group = await _db.Groups.FindAsync(request.Id);
        if (group == null)
        {
            return null;
        }

        var courseExists = await _db.Courses.AnyAsync(c => c.Id == request.CourseId);
        if (!courseExists)
        {
            throw new InvalidOperationException("Курс не найден.");
        }

        group.Name = request.Name;
        group.Curator = request.Curator;
        group.CourseId = request.CourseId;

        await _db.SaveChangesAsync();
        await _db.Entry(group).Reference(g => g.Course).LoadAsync();
        await _db.Entry(group).Collection(g => g.Students).LoadAsync();
        return ToDto(group);
    }

    public async Task<bool> DeleteGroupAsync(int id)
    {
        var group = await _db.Groups.Include(g => g.Students).FirstOrDefaultAsync(g => g.Id == id);
        if (group == null)
        {
            return false;
        }

        if (group.Students.Count > 0)
        {
            throw new InvalidOperationException("Нельзя удалить группу, в которой есть студенты.");
        }

        _db.Groups.Remove(group);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<GroupDto?> AssignCuratorAsync(int groupId, string curator)
    {
        var group = await _db.Groups
            .Include(g => g.Course)
            .Include(g => g.Students)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            return null;
        }

        group.Curator = curator;
        await _db.SaveChangesAsync();
        return ToDto(group);
    }

    public async Task AddStudentToGroupAsync(int studentId, int groupId)
    {
        var student = await _db.Students.FindAsync(studentId)
            ?? throw new InvalidOperationException("Студент не найден.");

        var groupExists = await _db.Groups.AnyAsync(g => g.Id == groupId);
        if (!groupExists)
        {
            throw new InvalidOperationException("Группа не найдена.");
        }

        student.GroupId = groupId;
        if (student.Status.StartsWith("Отчислен", StringComparison.OrdinalIgnoreCase))
        {
            student.Status = "Обучается";
        }

        await _db.SaveChangesAsync();
    }

    private static GroupDto ToDto(Group g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        Curator = g.Curator,
        CourseId = g.CourseId,
        CourseName = g.Course?.Name,
        StudentsCount = g.Students?.Count ?? 0
    };
}
