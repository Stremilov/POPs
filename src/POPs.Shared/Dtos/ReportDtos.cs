namespace POPs.Shared.Dtos;

public class ContingentItemDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
}

public class GroupReportDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Curator { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public bool IsEmpty { get; set; }
    public int TotalStudents { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public List<StudentDto> Students { get; set; } = new();
}

public class CourseStatisticsDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int GroupsCount { get; set; }
    public int StudentsCount { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new();
}

public class OverviewStatisticsDto
{
    public int TotalStudents { get; set; }
    public int TotalGroups { get; set; }
    public int TotalCourses { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new();
}
