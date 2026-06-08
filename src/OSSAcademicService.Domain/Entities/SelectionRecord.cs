using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 选课记录
/// </summary>
public class SelectionRecord : IAggregateRoot
{
    public long RecordId { get; private set; }
    public long StudentId { get; private set; }
    public long TaskId { get; private set; }
    public long CourseId { get; private set; }
    public long RoundId { get; private set; }
    public long SemesterId { get; private set; }
    public string Status { get; private set; }
    public bool IsLottery { get; private set; }
    public DateTime SelectedAt { get; private set; }
    public DateTime? DroppedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private SelectionRecord() { }

    public static SelectionRecord Create(long studentId, long taskId, long courseId,
        long roundId, long semesterId, bool isLottery = false)
    {
        return new SelectionRecord
        {
            StudentId = studentId,
            TaskId = taskId,
            CourseId = courseId,
            RoundId = roundId,
            SemesterId = semesterId,
            Status = "已选",
            IsLottery = isLottery,
            SelectedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Drop()
    {
        if (Status == "已退")
            throw new DomainException("课程已退选，不可重复退选");
        Status = "已退";
        DroppedAt = DateTime.UtcNow;
    }

    public void SetStatus(string status)
    {
        Status = status;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}