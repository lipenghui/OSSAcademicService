using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Events;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 教室借用聚合根
/// </summary>
public class ClassroomBooking : IAggregateRoot
{
    public long BookingId { get; private set; }
    public long ClassroomId { get; private set; }
    public long ApplicantId { get; private set; }
    public DateOnly BookingDate { get; private set; }
    public int StartPeriod { get; private set; }
    public int EndPeriod { get; private set; }
    public string Purpose { get; private set; }
    public int? AttendeeCount { get; private set; }
    public ApplyStatus ApplyStatus { get; private set; }
    public long? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? RejectReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private ClassroomBooking() { }

    public static ClassroomBooking Create(long classroomId, long applicantId,
        DateOnly bookingDate, int startPeriod, int endPeriod, string purpose, int? attendeeCount = null)
    {
        return new ClassroomBooking
        {
            ClassroomId = classroomId,
            ApplicantId = applicantId,
            BookingDate = bookingDate,
            StartPeriod = startPeriod,
            EndPeriod = endPeriod,
            Purpose = purpose,
            AttendeeCount = attendeeCount,
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
        AddDomainEvent(new ClassroomBookedEvent(ClassroomId, ApplicantId, BookingDate.ToDateTime(TimeOnly.MinValue), BookingDate.ToDateTime(TimeOnly.MaxValue)));
    }

    public void Reject(long rejectedBy, string reason)
    {
        ApplyStatus = ApplyStatus.Rejected;
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        RejectReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}