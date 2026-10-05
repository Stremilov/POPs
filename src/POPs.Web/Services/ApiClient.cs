using System.Net.Http.Json;
using POPs.Shared.Dtos;

namespace POPs.Web.Services;

/// <summary>
/// Простой клиент для обращения к API.
/// </summary>
public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    // ---- Auth ----
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    // ---- Students ----
    public Task<List<StudentDto>?> GetStudentsAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search)
            ? "api/students"
            : $"api/students?search={Uri.EscapeDataString(search)}";
        return _http.GetFromJsonAsync<List<StudentDto>>(url);
    }

    public Task<StudentDto?> GetStudentAsync(int id)
        => _http.GetFromJsonAsync<StudentDto>($"api/students/{id}");

    public async Task<(bool Ok, string? Error, StudentDto? Data)> CreateStudentAsync(StudentCreateRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/students", request);
        return await ReadResultAsync<StudentDto>(response);
    }

    public async Task<(bool Ok, string? Error, StudentDto? Data)> UpdateStudentAsync(StudentUpdateRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/students/{request.Id}", request);
        return await ReadResultAsync<StudentDto>(response);
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/students/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<(bool Ok, string? Error, StudentDto? Data)> ExpelStudentAsync(int id, string reason)
    {
        var response = await _http.PostAsJsonAsync($"api/students/{id}/expel", new ExpelRequest { Reason = reason });
        return await ReadResultAsync<StudentDto>(response);
    }

    public async Task<(bool Ok, string? Error, StudentDto? Data)> UpdatePersonalAsync(int id, PersonalDataRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/students/{id}/personal", request);
        return await ReadResultAsync<StudentDto>(response);
    }

    // ---- Groups ----
    public Task<List<GroupDto>?> GetGroupsAsync()
        => _http.GetFromJsonAsync<List<GroupDto>>("api/groups");

    public async Task<(bool Ok, string? Error, GroupDto? Data)> CreateGroupAsync(GroupCreateRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/groups", request);
        return await ReadResultAsync<GroupDto>(response);
    }

    public async Task<(bool Ok, string? Error, GroupDto? Data)> UpdateGroupAsync(GroupUpdateRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/groups/{request.Id}", request);
        return await ReadResultAsync<GroupDto>(response);
    }

    public async Task<(bool Ok, string? Error)> DeleteGroupAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/groups/{id}");
        if (response.IsSuccessStatusCode) return (true, null);
        var err = await ReadErrorAsync(response);
        return (false, err);
    }

    public async Task<(bool Ok, string? Error, GroupDto? Data)> AssignCuratorAsync(int groupId, string curator)
    {
        var response = await _http.PostAsJsonAsync($"api/groups/{groupId}/curator",
            new AssignCuratorRequest { Curator = curator });
        return await ReadResultAsync<GroupDto>(response);
    }

    public async Task<(bool Ok, string? Error)> EnrollStudentAsync(int studentId, int groupId)
    {
        var response = await _http.PostAsJsonAsync("api/groups/enroll",
            new AddStudentToGroupRequest { StudentId = studentId, GroupId = groupId });
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ReadErrorAsync(response));
    }

    // ---- Courses ----
    public Task<List<CourseDto>?> GetCoursesAsync()
        => _http.GetFromJsonAsync<List<CourseDto>>("api/courses");

    public async Task<(bool Ok, string? Error, CourseDto? Data)> CreateCourseAsync(CourseCreateRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/courses", request);
        return await ReadResultAsync<CourseDto>(response);
    }

    public async Task<(bool Ok, string? Error, CourseDto? Data)> UpdateCourseAsync(CourseUpdateRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/courses/{request.Id}", request);
        return await ReadResultAsync<CourseDto>(response);
    }

    public async Task<(bool Ok, string? Error)> AttachGroupAsync(int groupId, int courseId)
    {
        var response = await _http.PostAsJsonAsync("api/courses/attach-group",
            new AddGroupToCourseRequest { GroupId = groupId, CourseId = courseId });
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await ReadErrorAsync(response));
    }

    public async Task<(bool Ok, string? Message)> PromoteAsync(int courseId)
    {
        var response = await _http.PostAsync($"api/courses/{courseId}/promote", null);
        if (!response.IsSuccessStatusCode)
        {
            return (false, await ReadErrorAsync(response));
        }

        var payload = await response.Content.ReadFromJsonAsync<MessageResponse>();
        return (true, payload?.Message ?? "Готово");
    }

    // ---- Reports ----
    public Task<List<ContingentItemDto>?> GetContingentAsync()
        => _http.GetFromJsonAsync<List<ContingentItemDto>>("api/reports/contingent");

    public Task<OverviewStatisticsDto?> GetOverviewAsync()
        => _http.GetFromJsonAsync<OverviewStatisticsDto>("api/reports/overview");

    public Task<GroupReportDto?> GetGroupReportAsync(int groupId)
        => _http.GetFromJsonAsync<GroupReportDto>($"api/reports/group/{groupId}");

    public Task<CourseStatisticsDto?> GetCourseReportAsync(int courseId)
        => _http.GetFromJsonAsync<CourseStatisticsDto>($"api/reports/course/{courseId}");

    private static async Task<(bool Ok, string? Error, T? Data)> ReadResultAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>();
            return (true, null, data);
        }

        return (false, await ReadErrorAsync(response), default);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try
        {
            var err = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            if (!string.IsNullOrWhiteSpace(err?.Message))
            {
                return err.Message;
            }
        }
        catch
        {
            // ignore
        }

        return $"Ошибка HTTP {(int)response.StatusCode}";
    }

    private class ErrorResponse
    {
        public string? Message { get; set; }
    }

    private class MessageResponse
    {
        public string? Message { get; set; }
    }
}
