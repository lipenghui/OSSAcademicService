using Microsoft.Extensions.Logging;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Exceptions;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 成绩服务实现
/// </summary>
public class ScoreService : IScoreService
{
    private readonly IScoreRepository _scoreRepo;
    private readonly ISelectionRepository _selectionRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly ISemesterRepository _semesterRepo;
    private readonly ITeachingTaskRepository _taskRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ScoreService> _logger;

    public ScoreService(
        IScoreRepository scoreRepo,
        ISelectionRepository selectionRepo,
        ICourseRepository courseRepo,
        ISemesterRepository semesterRepo,
        ITeachingTaskRepository taskRepo,
        IUnitOfWork unitOfWork,
        ILogger<ScoreService> logger)
    {
        _scoreRepo = scoreRepo;
        _selectionRepo = selectionRepo;
        _courseRepo = courseRepo;
        _semesterRepo = semesterRepo;
        _taskRepo = taskRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<ScoreRecordDto>> GetTaskScoresAsync(long taskId, int page, int pageSize)
    {
        var (items, totalCount) = await _scoreRepo.GetPagedAsync(taskId, null, null, page, pageSize);

        var dtos = new List<ScoreRecordDto>();
        foreach (var item in items)
        {
            var course = await _courseRepo.GetByIdAsync(item.CourseId);
            dtos.Add(new ScoreRecordDto
            {
                RecordId = item.RecordId,
                StudentId = item.StudentId,
                CourseId = item.CourseId,
                CourseName = course?.CourseName ?? "",
                TaskId = item.TaskId,
                SemesterId = item.SemesterId,
                RegularScore = item.RegularScore,
                ExperimentScore = item.ExperimentScore,
                MidScore = item.MidScore,
                FinalScore = item.FinalScore,
                TotalScore = item.TotalScore,
                Grade = item.Grade,
                Gpa = item.Gpa,
                Credit = item.Credit,
                IsPassed = item.IsPassed,
                IsPublished = item.IsPublished
            });
        }

        return new PagedResult<ScoreRecordDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<Result> EnterScoreAsync(long studentId, long taskId, EnterScoreDto dto, long enteredBy)
    {
        var record = await _scoreRepo.GetByStudentAndTaskAsync(studentId, taskId);
        if (record == null)
            return Result.Failure("成绩记录不存在", 40401);

        if (record.IsPublished)
            return Result.Failure("成绩已发布，不可修改", 40301);

        try
        {
            var rule = await _scoreRepo.GetRuleByCourseAsync(record.CourseId);
            record.EnterScores(dto.Regular, dto.Experiment, dto.Mid, dto.Final, enteredBy, rule);
            _scoreRepo.Update(record);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, 40000);
        }
    }

    public async Task<Result> BatchEnterScoresAsync(long taskId, IReadOnlyList<BatchScoreEntryDto> entries, long enteredBy)
    {
        var rule = await GetRuleForTask(taskId);

        foreach (var entry in entries)
        {
            var record = await _scoreRepo.GetByStudentAndTaskAsync(entry.StudentId, taskId);
            if (record == null || record.IsPublished) continue;

            try
            {
                record.EnterScores(entry.Regular, entry.Experiment, entry.Mid, entry.Final, enteredBy, rule);
                _scoreRepo.Update(record);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("成绩录入失败: StudentId={StudentId}, Error={Error}", entry.StudentId, ex.Message);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> PublishScoresAsync(long taskId, long reviewedBy)
    {
        var records = await _scoreRepo.GetByTaskAsync(taskId);
        if (!records.Any())
            return Result.Failure("无成绩记录", 40401);

        if (records.Any(r => r.TotalScore == null))
            return Result.Failure("存在未录入完成的成绩", 42201);

        foreach (var record in records)
        {
            try
            {
                record.Publish(reviewedBy);
                _scoreRepo.Update(record);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("成绩发布失败: RecordId={RecordId}, Error={Error}", record.RecordId, ex.Message);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        var semesterId = records.First().SemesterId;
        _logger.LogInformation("成绩已发布: TaskId={TaskId}, SemesterId={SemesterId}", taskId, semesterId);
        return Result.Success();
    }

    public async Task<StudentTranscriptDto> GetTranscriptAsync(long studentId, long? semesterId)
    {
        var scores = await _scoreRepo.GetByStudentAsync(studentId, semesterId);
        var published = scores.Where(s => s.IsPublished).ToList();

        var semesterGroups = published
            .GroupBy(s => s.SemesterId)
            .Select(g => new SemesterTranscriptDto
            {
                SemesterId = g.Key,
                SemesterName = "",
                Records = g.Select(s => new TranscriptRecordDto
                {
                    CourseId = s.CourseId,
                    CourseName = "",
                    Credit = s.Credit,
                    TotalScore = s.TotalScore,
                    Gpa = s.Gpa,
                    Grade = s.Grade
                }).ToList(),
                SemesterGpa = CalculateGpa(g.ToList()),
                SemesterCredits = g.Sum(s => s.Credit)
            }).ToList();

        foreach (var group in semesterGroups)
        {
            var semester = await _semesterRepo.GetByIdAsync(group.SemesterId);
            group.SemesterName = semester?.SemesterName ?? "";

            foreach (var record in group.Records)
            {
                var course = await _courseRepo.GetByIdAsync(record.CourseId);
                record.CourseName = course?.CourseName ?? "";
            }
        }

        return new StudentTranscriptDto
        {
            StudentId = studentId,
            Semesters = semesterGroups,
            CumulativeGpa = CalculateGpa(published),
            TotalCredits = published.Sum(s => s.Credit)
        };
    }

    public async Task<GpaSummaryDto> GetGpaAsync(long studentId, long? semesterId)
    {
        var summary = await _scoreRepo.GetGpaSummaryAsync(studentId, semesterId);
        if (summary == null)
        {
            var scores = await _scoreRepo.GetByStudentAsync(studentId, semesterId);
            var published = scores.Where(s => s.IsPublished && s.Gpa.HasValue).ToList();

            var totalCredits = published.Sum(s => s.Credit);
            var totalPoints = published.Sum(s => s.Credit * s.Gpa!.Value);
            var gpa = totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;

            return new GpaSummaryDto
            {
                StudentId = studentId,
                SemesterId = semesterId,
                TotalCredits = totalCredits,
                TotalPoints = totalPoints,
                Gpa = gpa
            };
        }

        return new GpaSummaryDto
        {
            StudentId = summary.StudentId,
            SemesterId = summary.SemesterId,
            TotalCredits = summary.TotalCredits,
            TotalPoints = summary.TotalPoints,
            Gpa = summary.Gpa,
            MajorRank = summary.MajorRank,
            ClassRank = summary.ClassRank
        };
    }

    public async Task<ScoreAnalysisDto> GetAnalysisAsync(long taskId)
    {
        var scores = await _scoreRepo.GetByTaskAsync(taskId);
        if (!scores.Any())
            return new ScoreAnalysisDto { TaskId = taskId };

        var published = scores.Where(s => s.IsPublished).ToList();
        var totalScores = published.Where(s => s.TotalScore.HasValue).Select(s => s.TotalScore!.Value).ToList();

        return new ScoreAnalysisDto
        {
            TaskId = taskId,
            TotalStudents = scores.Count,
            PublishedCount = published.Count,
            AverageScore = totalScores.Any() ? Math.Round(totalScores.Average(), 2) : null,
            MaxScore = totalScores.Any() ? totalScores.Max() : null,
            MinScore = totalScores.Any() ? totalScores.Min() : null,
            StandardDeviation = CalculateStdDev(totalScores),
            PassRate = totalScores.Any() ? Math.Round((double)totalScores.Count(s => s >= 60) / totalScores.Count * 100, 2) : null,
            Distribution = new ScoreDistributionDto
            {
                Excellent = totalScores.Count(s => s >= 90),
                Good = totalScores.Count(s => s >= 80 && s < 90),
                Medium = totalScores.Count(s => s >= 70 && s < 80),
                Pass = totalScores.Count(s => s >= 60 && s < 70),
                Fail = totalScores.Count(s => s < 60)
            }
        };
    }

    private async Task<ScoreRule?> GetRuleForTask(long taskId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task == null) return null;
        return await _scoreRepo.GetRuleByCourseAsync(task.CourseId);
    }

    private static decimal? CalculateGpa(List<ScoreRecord> records)
    {
        var withGpa = records.Where(s => s.Gpa.HasValue).ToList();
        if (!withGpa.Any()) return null;

        var totalCredits = withGpa.Sum(s => s.Credit);
        var totalPoints = withGpa.Sum(s => s.Credit * s.Gpa!.Value);
        return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
    }

    private static double? CalculateStdDev(List<decimal> values)
    {
        if (values.Count < 2) return null;
        var avg = values.Average();
        var sumSquares = values.Sum(v => (double)((v - avg) * (v - avg)));
        return Math.Round(Math.Sqrt(sumSquares / (values.Count - 1)), 2);
    }
}