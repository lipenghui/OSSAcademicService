using OSSAcademicService.Domain.Events;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 考试安排聚合根
/// </summary>
public class ExamArrangement : IAggregateRoot
{
    public long ArrangementId { get; private set; }
    public long CourseId { get; private set; }
    public long TaskId { get; private set; }
    public long SemesterId { get; private set; }
    public string ExamType { get; private set; }
    public DateOnly ExamDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public long ClassroomId { get; private set; }
    public string ExamForm { get; private set; }
    public int? MaxStudents { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private ExamArrangement() { }

    public static ExamArrangement Create(
        long courseId, long taskId, long semesterId, string examType,
        DateOnly examDate, TimeOnly startTime, TimeOnly endTime,
        long classroomId, string examForm = "笔试")
    {
        return new ExamArrangement
        {
            CourseId = courseId,
            TaskId = taskId,
            SemesterId = semesterId,
            ExamType = examType,
            ExamDate = examDate,
            StartTime = startTime,
            EndTime = endTime,
            ClassroomId = classroomId,
            ExamForm = examForm,
            Status = "草稿",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Publish()
    {
        Status = "已发布";
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ExamArrangedEvent(ArrangementId, CourseId, ExamDate));
    }

    public void Cancel()
    {
        Status = "已取消";
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}