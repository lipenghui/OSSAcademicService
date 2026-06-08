using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Events;

/// <summary>
/// 学生入学事件
/// </summary>
public record StudentEnrolledEvent(long StudentId, string StudentNo, long UserId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 学籍状态变更事件
/// </summary>
public record StudentStatusChangedEvent(long StudentId, string StudentNo, string OldStatus, string NewStatus, string Reason) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 学生毕业事件
/// </summary>
public record StudentGraduatedEvent(long StudentId, string StudentNo, string CertificateNo) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 选课变更事件
/// </summary>
public record SelectionChangedEvent(long StudentId, long TaskId, long CourseId, string Action) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 成绩发布事件
/// </summary>
public record ScorePublishedEvent(long StudentId, long CourseId, long SemesterId, decimal? TotalScore) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 课程排课事件
/// </summary>
public record CourseScheduledEvent(long SemesterId, int ScheduledCount) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 调课事件
/// </summary>
public record ScheduleAdjustedEvent(long ItemId, string AdjustType, string Reason, long AppliedBy) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 考试安排事件
/// </summary>
public record ExamArrangedEvent(long ArrangementId, long CourseId, DateOnly ExamDate) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 教室借用事件
/// </summary>
public record ClassroomBookedEvent(long ClassroomId, long BookedBy, DateTime StartTime, DateTime EndTime) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

/// <summary>
/// 毕业审核完成事件
/// </summary>
public record GraduationAuditCompletedEvent(long StudentId, string Result, string? CertificateNo) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}