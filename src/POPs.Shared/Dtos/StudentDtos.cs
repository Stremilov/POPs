namespace POPs.Shared.Dtos;

public class StudentDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string? GroupName { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "Обучается";
    public string PreviousEducation { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string Login { get; set; } = string.Empty;
}

public class StudentCreateRequest
{
    public int GroupId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "Обучается";
    public string PreviousEducation { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = "student";
}

public class StudentUpdateRequest
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = "Обучается";
    public string PreviousEducation { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}

public class ExpelRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class PersonalDataRequest
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PreviousEducation { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}
