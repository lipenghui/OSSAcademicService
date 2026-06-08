namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 成绩权重规则
/// </summary>
public class ScoreRule
{
    public long RuleId { get; private set; }
    public long CourseId { get; private set; }
    public decimal RegularWeight { get; private set; }
    public decimal ExperimentWeight { get; private set; }
    public decimal MidWeight { get; private set; }
    public decimal FinalWeight { get; private set; }
    public string ScoreType { get; private set; }
    public string? GpaMapping { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ScoreRule() { }

    public static ScoreRule Create(long courseId, decimal regularWeight, decimal experimentWeight,
        decimal midWeight, decimal finalWeight, string scoreType = "百分制")
    {
        return new ScoreRule
        {
            CourseId = courseId,
            RegularWeight = regularWeight,
            ExperimentWeight = experimentWeight,
            MidWeight = midWeight,
            FinalWeight = finalWeight,
            ScoreType = scoreType,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(decimal regularWeight, decimal experimentWeight,
        decimal midWeight, decimal finalWeight, string scoreType)
    {
        RegularWeight = regularWeight;
        ExperimentWeight = experimentWeight;
        MidWeight = midWeight;
        FinalWeight = finalWeight;
        ScoreType = scoreType;
        UpdatedAt = DateTime.UtcNow;
    }
}