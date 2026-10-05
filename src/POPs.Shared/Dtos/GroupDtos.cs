namespace POPs.Shared.Dtos;

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Curator { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string? CourseName { get; set; }
    public int StudentsCount { get; set; }
}

public class GroupCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Curator { get; set; } = string.Empty;
    public int CourseId { get; set; }
}

public class GroupUpdateRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Curator { get; set; } = string.Empty;
    public int CourseId { get; set; }
}

public class AssignCuratorRequest
{
    public string Curator { get; set; } = string.Empty;
}

public class AddStudentToGroupRequest
{
    public int StudentId { get; set; }
    public int GroupId { get; set; }
}
