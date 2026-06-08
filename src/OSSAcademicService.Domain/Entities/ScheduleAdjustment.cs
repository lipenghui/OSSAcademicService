using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Events;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 调停补课聚合根
/// </summary>
public class ScheduleAdjustment : IAggregateRoot
{
    public long AdjustId { get; private set; }
    public long OriginalItemId { get; private set; }
    public AdjustType AdjustType { get; private set; }
    public DateOnly OriginalDate { get; private set; }
    public string? OriginalPeriod { get; private set; }
    public DateOnly? NewDate { get; private set; }
    public string? NewPeriod { get; private set; }
    public long? NewClassroomId { get; private set; }
    public string Reason { get; private set; }
    public ApplyStatus ApplyStatus { get; private set; }
    public long? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public bool NotifyStatus { get; private set; }
    public long CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private ScheduleAdjustment() { }

    public static ScheduleAdjustment Create(
        long originalItemId, AdjustType adjustType, DateOnly originalDate,
        string reason, long createdBy, DateOnly? newDate = null,
        string? newPeriod = null, long? newClassroomId = null)
    {
        return new ScheduleAdjustment
        {
            OriginalItemId = originalItemId,
            AdjustType = adjustType,
            OriginalDate = originalDate,
            Reason = reason,
            CreatedBy = createdBy,
            NewDate = newDate,
            NewPeriod = newPeriod,
            NewClassroomId = newClassroomId,
            ApplyStatus = ApplyStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Approve(long approvedBy)
    {
        ApplyStatus = ApplyStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ScheduleAdjustedEvent(OriginalItemId, AdjustType.ToString(), Reason, CreatedBy));
    }

    public void Reject(long rejectedBy)
    {
        ApplyStatus = ApplyStatus.Rejected;
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}