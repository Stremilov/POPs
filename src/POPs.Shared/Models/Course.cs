namespace POPs.Shared.Models;

/// <summary>
/// Курс обучения.
/// </summary>
public class Course
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int EndYear { get; set; }

    // Навигация: у курса много групп
    public List<Group> Groups { get; set; } = new();
}
