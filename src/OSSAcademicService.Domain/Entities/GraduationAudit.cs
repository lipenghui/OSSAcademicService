using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Events;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 毕业审核聚合根
/// </summary>
public class GraduationAudit : IAggregateRoot
{
    public long AuditId { get; private set; }
    public long StudentId { get; private set; }
    public long SemesterId { get; private set; }
    public decimal TotalCredits { get; private set; }
    public decimal RequiredCredits { get; private set; }
    public string? RequiredCourses { get; private set; }
    public decimal ElectiveCredits { get; private set; }
    public decimal? Gpa { get; private set; }
    public bool DisciplinaryCheck { get; private set; }
    public GraduationResult Result { get; private set; }
    public string? CertificateNo { get; private set; }
    public string? DegreeCertNo { get; private set; }
    public long? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private GraduationAudit() { }

    public static GraduationAudit Create(long studentId, long semesterId, decimal requiredCredits)
    {
        return new GraduationAudit
        {
            StudentId = studentId,
            SemesterId = semesterId,
            RequiredCredits = requiredCredits,
            DisciplinaryCheck = true,
            Result = GraduationResult.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void CompleteAudit(GraduationResult result, long reviewedBy, string? certificateNo = null, string? degreeCertNo = null)
    {
        Result = result;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        CertificateNo = certificateNo;
        DegreeCertNo = degreeCertNo;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new GraduationAuditCompletedEvent(StudentId, result.ToString(), certificateNo));
    }

    public void SetCredits(decimal totalCredits, decimal electiveCredits, decimal? gpa)
    {
        TotalCredits = totalCredits;
        ElectiveCredits = electiveCredits;
        Gpa = gpa;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}