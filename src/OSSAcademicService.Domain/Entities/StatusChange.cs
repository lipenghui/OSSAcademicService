using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 学籍异动
/// </summary>
public class StatusChange : IAggregateRoot
{
    public long ChangeId { get; private set; }
    public long StudentId { get; private set; }
    public ChangeType ChangeType { get; private set; }
    public string ChangeReason { get; private set; }
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public ApplyStatus ApplyStatus { get; private set; }
    public long? AppliedBy { get; private set; }
    public DateTime? AppliedAt { get; private set; }
    public DateOnly? EffectiveDate { get; private set; }
    public string? Attachments { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private StatusChange() { }

    public static StatusChange Create(long studentId, ChangeType changeType, string reason, long? appliedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("异动原因不能为空");

        return new StatusChange
        {
            StudentId = studentId,
            ChangeType = changeType,
            ChangeReason = reason.Trim(),
            ApplyStatus = ApplyStatus.Pending,
            AppliedBy = appliedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Approve(long approvedBy)
    {
        ApplyStatus = ApplyStatus.Approved;
        AppliedBy = approvedBy;
        AppliedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(long rejectedBy, string reason)
    {
        ApplyStatus = ApplyStatus.Rejected;
        AppliedBy = rejectedBy;
        AppliedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEffectiveDate(DateOnly date)
    {
        EffectiveDate = date;
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