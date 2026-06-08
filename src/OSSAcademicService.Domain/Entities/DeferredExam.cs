using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 缓考申请聚合根
/// </summary>
public class DeferredExam : IAggregateRoot
{
    public long DeferredId { get; private set; }
    public long StudentId { get; private set; }
    public long ArrangementId { get; private set; }
    public string DeferType { get; private set; }
    public string Reason { get; private set; }
    public string? ProofUrl { get; private set; }
    public ApplyStatus ApplyStatus { get; private set; }
    public long? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public long? NewArrangementId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private DeferredExam() { }

    public static DeferredExam Create(long studentId, long arrangementId, string reason, string? proofUrl = null)
    {
        return new DeferredExam
        {
            StudentId = studentId,
            ArrangementId = arrangementId,
            DeferType = "缓考",
            Reason = reason,
            ProofUrl = proofUrl,
            ApplyStatus = ApplyStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Approve(long approvedBy, long newArrangementId)
    {
        ApplyStatus = ApplyStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        NewArrangementId = newArrangementId;
        UpdatedAt = DateTime.UtcNow;
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