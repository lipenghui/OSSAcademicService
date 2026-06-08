namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 培养方案课程关联
/// </summary>
public class PlanCourse
{
    public long Id { get; private set; }
    public long PlanId { get; private set; }
    public long CourseId { get; private set; }
    public int SemesterNo { get; private set; }
    public decimal CreditRequired { get; private set; }
    public bool IsCompulsory { get; private set; }
    public string Platform { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PlanCourse(long planId, long courseId, int semesterNo,
        decimal creditRequired, bool isCompulsory, string platform)
    {
        PlanId = planId;
        CourseId = courseId;
        SemesterNo = semesterNo;
        CreditRequired = creditRequired;
        IsCompulsory = isCompulsory;
        Platform = platform;
        CreatedAt = DateTime.UtcNow;
    }

    private PlanCourse() { }
}