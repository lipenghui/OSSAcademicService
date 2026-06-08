namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 学期
/// </summary>
public class Semester
{
    public long SemesterId { get; private set; }
    public string SemesterName { get; private set; }
    public string AcademicYear { get; private set; }
    public int Term { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public DateOnly TeachStart { get; private set; }
    public DateOnly TeachEnd { get; private set; }
    public int TotalWeeks { get; private set; }
    public bool IsCurrent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Semester() { }

    public static Semester Create(
        string semesterName, string academicYear, int term,
        DateOnly startDate, DateOnly endDate, DateOnly teachStart, DateOnly teachEnd,
        int totalWeeks = 18)
    {
        return new Semester
        {
            SemesterName = semesterName,
            AcademicYear = academicYear,
            Term = term,
            StartDate = startDate,
            EndDate = endDate,
            TeachStart = teachStart,
            TeachEnd = teachEnd,
            TotalWeeks = totalWeeks,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void SetCurrent()
    {
        IsCurrent = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnsetCurrent()
    {
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }
}