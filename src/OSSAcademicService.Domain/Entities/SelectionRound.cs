using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 选课轮次聚合根
/// </summary>
public class SelectionRound : IAggregateRoot
{
    public long RoundId { get; private set; }
    public long SemesterId { get; private set; }
    public SelectionRoundType RoundType { get; private set; }
    public string RoundName { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Rules { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private SelectionRound() { }

    public static SelectionRound Create(
        long semesterId, SelectionRoundType roundType, string roundName,
        DateTime startTime, DateTime endTime, string rules)
    {
        return new SelectionRound
        {
            SemesterId = semesterId,
            RoundType = roundType,
            RoundName = roundName,
            StartTime = startTime,
            EndTime = endTime,
            Rules = rules,
            Status = "未开始",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Open()
    {
        Status = "进行中";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Close()
    {
        Status = "已结束";
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive() => Status == "进行中" && DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}