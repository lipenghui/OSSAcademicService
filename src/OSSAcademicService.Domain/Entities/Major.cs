namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 专业
/// </summary>
public class Major
{
    public long MajorId { get; private set; }
    public string MajorCode { get; private set; }
    public string MajorName { get; private set; }
    public long CollegeId { get; private set; }
    public string EduLevel { get; private set; }
    public int LengthOfSchool { get; private set; }
    public string? DegreeType { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Major() { }

    public static Major Create(string majorCode, string majorName, long collegeId,
        string eduLevel = "本科", int lengthOfSchool = 4, string? degreeType = null)
    {
        return new Major
        {
            MajorCode = majorCode,
            MajorName = majorName,
            CollegeId = collegeId,
            EduLevel = eduLevel,
            LengthOfSchool = lengthOfSchool,
            DegreeType = degreeType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}