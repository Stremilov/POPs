namespace POPs.Shared.Dtos;

public class CourseDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
    public int GroupsCount { get; set; }
}

public class CourseCreateRequest
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
}

public class CourseUpdateRequest
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }
}

public class AddGroupToCourseRequest
{
    public int GroupId { get; set; }
    public int CourseId { get; set; }
}
