using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 教学任务聚合根
/// </summary>
public class TeachingTask : IAggregateRoot
{
    public long TaskId { get; private set; }
    public long CourseId { get; private set; }
    public long TeacherId { get; private set; }
    public long SemesterId { get; private set; }
    public string ClassIds { get; private set; }
    public int? MaxStudents { get; private set; }
    public int CurrentCount { get; private set; }
    public decimal WeeklyHours { get; private set; }
    public int TotalWeeks { get; private set; }
    public string? Remark { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private TeachingTask() { }

    public static TeachingTask Create(
        long courseId, long teacherId, long semesterId,
        string classIds, int? maxStudents, decimal weeklyHours, int totalWeeks = 18)
    {
        return new TeachingTask
        {
            CourseId = courseId,
            TeacherId = teacherId,
            SemesterId = semesterId,
            ClassIds = classIds,
            MaxStudents = maxStudents,
            WeeklyHours = weeklyHours,
            TotalWeeks = totalWeeks,
            CurrentCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void IncrementCount()
    {
        CurrentCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementCount()
    {
        if (CurrentCount > 0) CurrentCount--;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsCapacityFull() => MaxStudents.HasValue && CurrentCount >= MaxStudents.Value;

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}