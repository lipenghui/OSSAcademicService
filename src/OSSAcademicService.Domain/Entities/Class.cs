namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 班级
/// </summary>
public class Class
{
    public long ClassId { get; private set; }
    public string ClassName { get; private set; }
    public string ClassCode { get; private set; }
    public long MajorId { get; private set; }
    public int GradeYear { get; private set; }
    public long CollegeId { get; private set; }
    public long? CounselorId { get; private set; }
    public long? HeadTeacherId { get; private set; }
    public int? MaxStudents { get; private set; }
    public int CurrentCount { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Class() { }

    public static Class Create(string className, string classCode, long majorId,
        int gradeYear, long collegeId, int? maxStudents = null)
    {
        return new Class
        {
            ClassName = className,
            ClassCode = classCode,
            MajorId = majorId,
            GradeYear = gradeYear,
            CollegeId = collegeId,
            MaxStudents = maxStudents,
            CurrentCount = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void IncrementCount()
    {
        CurrentCount++;
    }

    public void DecrementCount()
    {
        if (CurrentCount > 0) CurrentCount--;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}