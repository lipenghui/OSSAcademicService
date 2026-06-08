using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Events;
using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 学生档案聚合根
/// </summary>
public class StudentProfile : IAggregateRoot
{
    public long StudentId { get; private set; }
    public string StudentNo { get; private set; }
    public long UserId { get; private set; }
    public long CollegeId { get; private set; }
    public long MajorId { get; private set; }
    public long? ClassId { get; private set; }
    public int GradeYear { get; private set; }
    public string EduLevel { get; private set; }
    public int LengthOfSchool { get; private set; }
    public long? TrainingPlanId { get; private set; }
    public StudentStatus Status { get; private set; }
    public DateOnly? EnrollmentDate { get; private set; }
    public DateOnly? ExpectedGraduation { get; private set; }
    public DateOnly? ActualGraduation { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<StatusChange> _statusChanges = new();
    public IReadOnlyCollection<StatusChange> StatusChanges => _statusChanges.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private StudentProfile() { }

    public static StudentProfile Create(
        string studentNo, long userId, long collegeId, long majorId,
        int gradeYear, string eduLevel, int lengthOfSchool = 4)
    {
        if (string.IsNullOrWhiteSpace(studentNo))
            throw new DomainException("学号不能为空");

        var profile = new StudentProfile
        {
            StudentNo = studentNo.Trim(),
            UserId = userId,
            CollegeId = collegeId,
            MajorId = majorId,
            GradeYear = gradeYear,
            EduLevel = eduLevel,
            LengthOfSchool = lengthOfSchool,
            Status = StudentStatus.Enrolled,
            EnrollmentDate = DateOnly.FromDateTime(DateTime.UtcNow),
            ExpectedGraduation = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(lengthOfSchool)),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        profile.AddDomainEvent(new StudentEnrolledEvent(profile.StudentId, profile.StudentNo, profile.UserId));
        return profile;
    }

    public void ChangeStatus(StudentStatus newStatus, string reason, long? approvedBy = null)
    {
        var oldValue = Status.ToString();
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        var change = StatusChange.Create(StudentId, MapChangeType(newStatus), reason, approvedBy);
        _statusChanges.Add(change);

        AddDomainEvent(new StudentStatusChangedEvent(StudentId, StudentNo, oldValue, newStatus.ToString(), reason));
    }

    public void TransferMajor(long newMajorId, long newClassId, string reason, long approvedBy)
    {
        var oldMajorId = MajorId;
        var oldClassId = ClassId;
        MajorId = newMajorId;
        ClassId = newClassId;
        UpdatedAt = DateTime.UtcNow;

        var change = StatusChange.Create(StudentId, ChangeType.TransferMajor, reason, approvedBy);
        _statusChanges.Add(change);
    }

    public void Graduate(DateOnly graduationDate, string certificateNo)
    {
        Status = StudentStatus.Graduated;
        ActualGraduation = graduationDate;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new StudentGraduatedEvent(StudentId, StudentNo, certificateNo));
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsEligibleForSelection() =>
        Status == StudentStatus.Enrolled || Status == StudentStatus.Probation;

    private static ChangeType MapChangeType(StudentStatus status) => status switch
    {
        StudentStatus.Suspended => ChangeType.Suspension,
        StudentStatus.Withdrawn => ChangeType.Withdrawal,
        StudentStatus.Graduated => ChangeType.Graduation,
        _ => ChangeType.Other
    };

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}