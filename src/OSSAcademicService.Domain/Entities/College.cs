namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 院系
/// </summary>
public class College
{
    public long CollegeId { get; private set; }
    public string CollegeCode { get; private set; }
    public string CollegeName { get; private set; }
    public string? ShortName { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private College() { }

    public static College Create(string collegeCode, string collegeName, string? shortName = null, int sortOrder = 0)
    {
        return new College
        {
            CollegeCode = collegeCode,
            CollegeName = collegeName,
            ShortName = shortName,
            SortOrder = sortOrder,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}