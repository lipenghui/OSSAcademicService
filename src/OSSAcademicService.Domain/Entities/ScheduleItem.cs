using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 排课条目
/// </summary>
public class ScheduleItem : IAggregateRoot
{
    public long ItemId { get; private set; }
    public long TaskId { get; private set; }
    public long CourseId { get; private set; }
    public long TeacherId { get; private set; }
    public long ClassroomId { get; private set; }
    public long SemesterId { get; private set; }
    public int DayOfWeek { get; private set; }
    public int StartPeriod { get; private set; }
    public int EndPeriod { get; private set; }
    public int StartWeek { get; private set; }
    public int EndWeek { get; private set; }
    public string WeekType { get; private set; }
    public bool IsLocked { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private ScheduleItem() { }

    public static ScheduleItem Create(long taskId, long courseId, long teacherId,
        long classroomId, long semesterId, int dayOfWeek,
        int startPeriod, int endPeriod, int startWeek, int endWeek,
        string weekType = "每周")
    {
        ValidatePeriods(startPeriod, endPeriod);
        ValidateWeeks(startWeek, endWeek);

        return new ScheduleItem
        {
            TaskId = taskId,
            CourseId = courseId,
            TeacherId = teacherId,
            ClassroomId = classroomId,
            SemesterId = semesterId,
            DayOfWeek = dayOfWeek,
            StartPeriod = startPeriod,
            EndPeriod = endPeriod,
            StartWeek = startWeek,
            EndWeek = endWeek,
            WeekType = weekType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Lock() { IsLocked = true; UpdatedAt = DateTime.UtcNow; }
    public void Unlock() { IsLocked = false; UpdatedAt = DateTime.UtcNow; }

    public bool OverlapsWith(ScheduleItem other)
    {
        if (SemesterId != other.SemesterId) return false;
        if (DayOfWeek != other.DayOfWeek) return false;
        if (!WeeksOverlap(other)) return false;
        return PeriodsOverlap(other);
    }

    private bool WeeksOverlap(ScheduleItem other)
    {
        if (StartWeek > other.EndWeek || EndWeek < other.StartWeek) return false;
        if (WeekType == "每周" || other.WeekType == "每周") return true;
        if (WeekType == other.WeekType) return true;
        return true;
    }

    private bool PeriodsOverlap(ScheduleItem other) =>
        StartPeriod <= other.EndPeriod && EndPeriod >= other.StartPeriod;

    private static void ValidatePeriods(int start, int end)
    {
        if (start < 1 || start > 12) throw new DomainException("开始节次必须在1-12之间");
        if (end < start) throw new DomainException("结束节次不能小于开始节次");
        if (end > 12) throw new DomainException("结束节次不能超过12");
    }

    private static void ValidateWeeks(int start, int end)
    {
        if (start < 1 || start > 25) throw new DomainException("开始周次无效");
        if (end < start) throw new DomainException("结束周次不能小于开始周次");
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}