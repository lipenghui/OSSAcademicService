using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 培养方案聚合根
/// </summary>
public class TrainingPlan : IAggregateRoot
{
    public long PlanId { get; private set; }
    public long MajorId { get; private set; }
    public int GradeYear { get; private set; }
    public string PlanName { get; private set; }
    public decimal TotalCredits { get; private set; }
    public int LengthOfSchool { get; private set; }
    public string PlanStructure { get; private set; }
    public int Version { get; private set; }
    public bool IsActive { get; private set; }
    public long? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private TrainingPlan() { }

    public static TrainingPlan Create(
        long majorId, int gradeYear, string planName,
        decimal totalCredits, int lengthOfSchool, string planStructure)
    {
        if (string.IsNullOrWhiteSpace(planName))
            throw new DomainException("培养方案名称不能为空");

        return new TrainingPlan
        {
            MajorId = majorId,
            GradeYear = gradeYear,
            PlanName = planName.Trim(),
            TotalCredits = totalCredits,
            LengthOfSchool = lengthOfSchool,
            PlanStructure = planStructure,
            Version = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Approve(long approvedBy)
    {
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
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