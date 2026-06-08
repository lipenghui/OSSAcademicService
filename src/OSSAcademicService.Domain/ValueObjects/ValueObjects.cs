namespace OSSAcademicService.Domain.ValueObjects;

/// <summary>
/// 时间段值对象
/// </summary>
public record TimeSlot(int DayOfWeek, int StartPeriod, int EndPeriod, int StartWeek, int EndWeek, string WeekType)
{
    public bool OverlapsWith(TimeSlot other)
    {
        if (DayOfWeek != other.DayOfWeek) return false;
        if (StartWeek > other.EndWeek || EndWeek < other.StartWeek) return false;
        return StartPeriod <= other.EndPeriod && EndPeriod >= other.StartPeriod;
    }
}

/// <summary>
/// 排课约束值对象
/// </summary>
public record ScheduleConstraint
{
    public int MaxPeriodsPerDay { get; init; } = 8;
    public int MaxPeriodsPerWeek { get; init; } = 30;
    public List<int> AvailablePeriods { get; init; } = Enumerable.Range(1, 12).ToList();
    public List<int> AvailableDays { get; init; } = Enumerable.Range(1, 5).ToList();
    public int MinClassroomCapacityMargin { get; init; } = 10;
    public bool AvoidSameTeacherSamePeriod { get; init; } = true;
    public bool AvoidSameClassSamePeriod { get; init; } = true;
    public bool AvoidSameClassroomSamePeriod { get; init; } = true;
}