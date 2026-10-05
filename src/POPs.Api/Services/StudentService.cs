using Microsoft.EntityFrameworkCore;
using POPs.Api.Data;
using POPs.Shared.Dtos;
using POPs.Shared.Models;

namespace POPs.Api.Services;

/// <summary>
/// Сервис управления студентами (по диаграмме классов).
/// </summary>
public class StudentService
{
    private readonly AppDbContext _db;

    public StudentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<StudentDto>> GetAllAsync(string? search = null)
    {
        var query = _db.Students.Include(s => s.Group).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(s =>
                s.FullName.ToLower().Contains(search) ||
                s.Email.ToLower().Contains(search) ||
                s.Status.ToLower().Contains(search));
        }

        var list = await query.OrderBy(s => s.FullName).ToListAsync();
        return list.Select(ToDto).ToList();
    }

    public async Task<StudentDto?> FindStudentAsync(int id)
    {
        var student = await _db.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
        return student == null ? null : ToDto(student);
    }

    public async Task<List<StudentDto>> GetStudentsByGroupAsync(int groupId)
    {
        var list = await _db.Students
            .Include(s => s.Group)
            .Where(s => s.GroupId == groupId)
            .OrderBy(s => s.FullName)
            .ToListAsync();

        return list.Select(ToDto).ToList();
    }

    public async Task<StudentDto> AddStudentAsync(StudentCreateRequest request)
    {
        var groupExists = await _db.Groups.AnyAsync(g => g.Id == request.GroupId);
        if (!groupExists)
        {
            throw new InvalidOperationException("Группа не найдена.");
        }

        if (await _db.Students.AnyAsync(s => s.Login == request.Login))
        {
            throw new InvalidOperationException("Логин уже занят.");
        }

        var student = new Student
        {
            GroupId = request.GroupId,
            FullName = request.FullName,
            BirthDate = request.BirthDate,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Обучается" : request.Status,
            PreviousEducation = request.PreviousEducation,
            GraduationYear = request.GraduationYear,
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                string.IsNullOrWhiteSpace(request.Password) ? "student" : request.Password)
        };

        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        await _db.Entry(student).Reference(s => s.Group).LoadAsync();
        return ToDto(student);
    }

    public async Task<StudentDto?> EditStudentAsync(StudentUpdateRequest request)
    {
        var student = await _db.Students.FindAsync(request.Id);
        if (student == null)
        {
            return null;
        }

        var groupExists = await _db.Groups.AnyAsync(g => g.Id == request.GroupId);
        if (!groupExists)
        {
            throw new InvalidOperationException("Группа не найдена.");
        }

        student.GroupId = request.GroupId;
        student.FullName = request.FullName;
        student.BirthDate = request.BirthDate;
        student.Phone = request.Phone;
        student.Email = request.Email;
        student.Address = request.Address;
        student.Status = request.Status;
        student.PreviousEducation = request.PreviousEducation;
        student.GraduationYear = request.GraduationYear;

        await _db.SaveChangesAsync();
        await _db.Entry(student).Reference(s => s.Group).LoadAsync();
        return ToDto(student);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student == null)
        {
            return false;
        }

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<StudentDto?> ExpelStudentAsync(int id, string reason)
    {
        var student = await _db.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
        if (student == null)
        {
            return null;
        }

        student.Status = string.IsNullOrWhiteSpace(reason)
            ? "Отчислен"
            : $"Отчислен ({reason})";

        await _db.SaveChangesAsync();
        return ToDto(student);
    }

    public async Task<StudentDto?> UpdatePersonalDataAsync(int studentId, PersonalDataRequest request)
    {
        var student = await _db.Students.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == studentId);
        if (student == null)
        {
            return null;
        }

        student.FullName = request.FullName;
        student.BirthDate = request.BirthDate;
        student.Phone = request.Phone;
        student.Email = request.Email;
        student.Address = request.Address;
        student.PreviousEducation = request.PreviousEducation;
        student.GraduationYear = request.GraduationYear;

        await _db.SaveChangesAsync();
        return ToDto(student);
    }

    private static StudentDto ToDto(Student s) => new()
    {
        Id = s.Id,
        GroupId = s.GroupId,
        GroupName = s.Group?.Name,
        FullName = s.FullName,
        BirthDate = s.BirthDate,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        Status = s.Status,
        PreviousEducation = s.PreviousEducation,
        GraduationYear = s.GraduationYear,
        Login = s.Login
    };
}
