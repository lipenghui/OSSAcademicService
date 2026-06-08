using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 课程聚合根
/// </summary>
public class Course : IAggregateRoot
{
    public long CourseId { get; private set; }
    public string CourseCode { get; private set; }
    public string CourseName { get; private set; }
    public string? CourseNameEn { get; private set; }
    public decimal Credit { get; private set; }
    public int TotalHours { get; private set; }
    public int TeachingHours { get; private set; }
    public int LabHours { get; private set; }
    public int PracticeHours { get; private set; }
    public string TeachingMode { get; private set; }
    public string CourseType { get; private set; }
    public long CollegeId { get; private set; }
    public string? OutlineUrl { get; private set; }
    public int? MaxCapacity { get; private set; }
    public string AssessMethod { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<CoursePrerequisite> _prerequisites = new();
    public IReadOnlyCollection<CoursePrerequisite> Prerequisites => _prerequisites.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Course() { }

    public static Course Create(
        string courseCode, string courseName, decimal credit, int totalHours,
        string courseType, long collegeId, string teachingMode = "讲授")
    {
        if (string.IsNullOrWhiteSpace(courseCode))
            throw new DomainException("课程编号不能为空");
        if (string.IsNullOrWhiteSpace(courseName))
            throw new DomainException("课程名称不能为空");
        if (credit <= 0)
            throw new DomainException("学分必须大于0");

        return new Course
        {
            CourseCode = courseCode.Trim().ToUpperInvariant(),
            CourseName = courseName.Trim(),
            Credit = credit,
            TotalHours = totalHours,
            CourseType = courseType,
            CollegeId = collegeId,
            TeachingMode = teachingMode,
            AssessMethod = "考试",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string courseName, decimal credit, int totalHours,
        int teachingHours, int labHours, int practiceHours, string? outlineUrl)
    {
        CourseName = courseName.Trim();
        Credit = credit;
        TotalHours = totalHours;
        TeachingHours = teachingHours;
        LabHours = labHours;
        PracticeHours = practiceHours;
        OutlineUrl = outlineUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPrerequisite(long relatedCourseId, string relationType, bool isRequired = true)
    {
        if (_prerequisites.Any(p => p.RelatedCourseId == relatedCourseId && p.RelationType == relationType))
            return;

        _prerequisites.Add(new CoursePrerequisite(CourseId, relatedCourseId, relationType, isRequired));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}