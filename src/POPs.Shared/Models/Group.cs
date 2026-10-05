namespace POPs.Shared.Models;

/// <summary>
/// Учебная группа.
/// </summary>
public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Curator { get; set; } = string.Empty;
    public int CourseId { get; set; }

    // Навигация
    public Course? Course { get; set; }
    public List<Student> Students { get; set; } = new();
}
