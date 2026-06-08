namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// GPA 汇总
/// </summary>
public class GpaSummary
{
    public long SummaryId { get; private set; }
    public long StudentId { get; private set; }
    public long? SemesterId { get; private set; }
    public decimal TotalCredits { get; private set; }
    public decimal TotalPoints { get; private set; }
    public decimal Gpa { get; private set; }
    public int? MajorRank { get; private set; }
    public int? MajorTotal { get; private set; }
    public int? ClassRank { get; private set; }
    public int? ClassTotal { get; private set; }
    public DateTime CalculatedAt { get; private set; }

    private GpaSummary() { }

    public static GpaSummary Create(long studentId, long? semesterId,
        decimal totalCredits, decimal totalPoints, decimal gpa)
    {
        return new GpaSummary
        {
            StudentId = studentId,
            SemesterId = semesterId,
            TotalCredits = totalCredits,
            TotalPoints = totalPoints,
            Gpa = gpa,
            CalculatedAt = DateTime.UtcNow
        };
    }

    public void SetRanks(int? majorRank, int? majorTotal, int? classRank, int? classTotal)
    {
        MajorRank = majorRank;
        MajorTotal = majorTotal;
        ClassRank = classRank;
        ClassTotal = classTotal;
    }

    public void Update(decimal totalCredits, decimal totalPoints, decimal gpa)
    {
        TotalCredits = totalCredits;
        TotalPoints = totalPoints;
        Gpa = gpa;
        CalculatedAt = DateTime.UtcNow;
    }
}