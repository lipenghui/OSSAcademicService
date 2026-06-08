namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 课程先修/并修/替代关系
/// </summary>
public class CoursePrerequisite
{
    public long Id { get; private set; }
    public long CourseId { get; private set; }
    public long RelatedCourseId { get; private set; }
    public string RelationType { get; private set; }
    public bool IsRequired { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public CoursePrerequisite(long courseId, long relatedCourseId, string relationType, bool isRequired)
    {
        CourseId = courseId;
        RelatedCourseId = relatedCourseId;
        RelationType = relationType;
        IsRequired = isRequired;
        CreatedAt = DateTime.UtcNow;
    }

    private CoursePrerequisite() { }
}