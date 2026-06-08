using OSSAcademicService.Domain.Events;
using OSSAcademicService.Domain.Exceptions;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 成绩记录聚合根
/// </summary>
public class ScoreRecord : IAggregateRoot
{
    public long RecordId { get; private set; }
    public long StudentId { get; private set; }
    public long CourseId { get; private set; }
    public long TaskId { get; private set; }
    public long SemesterId { get; private set; }
    public decimal? RegularScore { get; private set; }
    public decimal? ExperimentScore { get; private set; }
    public decimal? FinalScore { get; private set; }
    public decimal? MidScore { get; private set; }
    public decimal? TotalScore { get; private set; }
    public string? Grade { get; private set; }
    public decimal? Gpa { get; private set; }
    public decimal Credit { get; private set; }
    public bool? IsPassed { get; private set; }
    public string ScoreType { get; private set; }
    public bool IsPublished { get; private set; }
    public long? EnteredBy { get; private set; }
    public DateTime? EnteredAt { get; private set; }
    public long? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? Remark { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private ScoreRecord() { }

    public static ScoreRecord Create(long studentId, long courseId, long taskId,
        long semesterId, decimal credit, string scoreType = "百分制")
    {
        return new ScoreRecord
        {
            StudentId = studentId,
            CourseId = courseId,
            TaskId = taskId,
            SemesterId = semesterId,
            Credit = credit,
            ScoreType = scoreType,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void EnterScores(decimal? regular, decimal? experiment, decimal? mid,
        decimal? final, long enteredBy, ScoreRule? rule = null)
    {
        RegularScore = ValidateScore(regular, nameof(regular));
        ExperimentScore = ValidateScore(experiment, nameof(experiment));
        MidScore = ValidateScore(mid, nameof(mid));
        FinalScore = ValidateScore(final, nameof(final));
        EnteredBy = enteredBy;
        EnteredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        if (rule != null)
            CalculateTotalScore(rule);
    }

    public void CalculateTotalScore(ScoreRule rule)
    {
        TotalScore = (RegularScore ?? 0) * rule.RegularWeight
                   + (ExperimentScore ?? 0) * rule.ExperimentWeight
                   + (MidScore ?? 0) * rule.MidWeight
                   + (FinalScore ?? 0) * rule.FinalWeight;

        TotalScore = Math.Round(TotalScore.Value, 1);

        switch (ScoreType)
        {
            case "百分制":
                Grade = null;
                IsPassed = TotalScore >= 60;
                Gpa = CalculateGpa(TotalScore.Value);
                break;
            case "五级制":
                Grade = TotalScore switch
                {
                    >= 90 => "优",
                    >= 80 => "良",
                    >= 70 => "中",
                    >= 60 => "及格",
                    _ => "不及格"
                };
                IsPassed = TotalScore >= 60;
                Gpa = CalculateGpa(TotalScore.Value);
                break;
            case "二级制":
                IsPassed = TotalScore >= 60;
                Grade = IsPassed.Value ? "通过" : "不通过";
                Gpa = IsPassed.Value ? 1.0m : 0m;
                break;
        }
    }

    public void Publish(long reviewedBy)
    {
        if (TotalScore == null)
            throw new DomainException("成绩未录入完成，无法发布");

        IsPublished = true;
        ReviewedBy = reviewedBy;
        ReviewedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new ScorePublishedEvent(StudentId, CourseId, SemesterId, TotalScore));
    }

    private static decimal? ValidateScore(decimal? score, string fieldName)
    {
        if (score == null) return null;
        if (score < 0 || score > 100)
            throw new DomainException($"{fieldName} 必须在 0-100 之间");
        return score;
    }

    private static decimal CalculateGpa(decimal score) => score switch
    {
        >= 90 => 4.0m,
        >= 85 => 3.7m,
        >= 82 => 3.3m,
        >= 78 => 3.0m,
        >= 75 => 2.7m,
        >= 72 => 2.3m,
        >= 68 => 2.0m,
        >= 64 => 1.5m,
        >= 60 => 1.0m,
        _ => 0m
    };

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}