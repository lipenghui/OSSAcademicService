using Microsoft.Extensions.Logging;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Exceptions;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 选课服务实现
/// </summary>
public class SelectionService : ISelectionService
{
    private readonly ISelectionRepository _selectionRepo;
    private readonly IStudentProfileRepository _studentRepo;
    private readonly ITeachingTaskRepository _taskRepo;
    private readonly IScheduleRepository _scheduleRepo;
    private readonly ISemesterRepository _semesterRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SelectionService> _logger;

    public SelectionService(
        ISelectionRepository selectionRepo,
        IStudentProfileRepository studentRepo,
        ITeachingTaskRepository taskRepo,
        IScheduleRepository scheduleRepo,
        ISemesterRepository semesterRepo,
        ICourseRepository courseRepo,
        IUnitOfWork unitOfWork,
        ILogger<SelectionService> logger)
    {
        _selectionRepo = selectionRepo;
        _studentRepo = studentRepo;
        _taskRepo = taskRepo;
        _scheduleRepo = scheduleRepo;
        _semesterRepo = semesterRepo;
        _courseRepo = courseRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SelectionRoundDto>> GetRoundsAsync(long semesterId)
    {
        var rounds = await _selectionRepo.GetRoundsBySemesterAsync(semesterId);
        return rounds.Select(r => new SelectionRoundDto
        {
            RoundId = r.RoundId,
            SemesterId = r.SemesterId,
            RoundType = r.RoundType.ToString(),
            RoundName = r.RoundName,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            Status = r.Status
        }).ToList();
    }

    public async Task<Result<long>> CreateRoundAsync(CreateSelectionRoundDto dto)
    {
        var roundType = Enum.Parse<SelectionRoundType>(dto.RoundType);
        var round = SelectionRound.Create(dto.SemesterId, roundType, dto.RoundName, dto.StartTime, dto.EndTime, dto.Rules);

        _selectionRepo.AddRound(round);
        await _unitOfWork.SaveChangesAsync();

        return Result<long>.Success(round.RoundId);
    }

    public async Task<Result> OpenRoundAsync(long roundId)
    {
        var round = await _selectionRepo.GetRoundByIdAsync(roundId);
        if (round == null)
            return Result.Failure("选课轮次不存在", 40401);

        round.Open();
        _selectionRepo.UpdateRound(round);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> CloseRoundAsync(long roundId)
    {
        var round = await _selectionRepo.GetRoundByIdAsync(roundId);
        if (round == null)
            return Result.Failure("选课轮次不存在", 40401);

        round.Close();
        _selectionRepo.UpdateRound(round);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<long>> SelectCourseAsync(long studentId, long taskId, long roundId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null || !student.IsEligibleForSelection())
            return Result<long>.Failure("学生状态不允许选课", 40301);

        var round = await _selectionRepo.GetRoundByIdAsync(roundId);
        if (round == null || !round.IsActive())
            return Result<long>.Failure("选课轮次未开放", 40302);

        if (await _selectionRepo.ExistsAsync(studentId, taskId))
            return Result<long>.Failure("已选择该课程", 40901);

        var task = await _taskRepo.GetByIdAsync(taskId);
        if (task == null)
            return Result<long>.Failure("教学任务不存在", 40401);

        if (task.IsCapacityFull())
            return Result<long>.Failure("课程容量已满", 40902);

        try
        {
            var record = SelectionRecord.Create(studentId, taskId, task.CourseId, roundId, task.SemesterId);
            _selectionRepo.Add(record);
            task.IncrementCount();
            _taskRepo.Update(task);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("选课成功: StudentId={StudentId}, TaskId={TaskId}", studentId, taskId);
            return Result<long>.Success(record.RecordId);
        }
        catch (DomainException ex)
        {
            return Result<long>.Failure(ex.Message, 40000);
        }
    }

    public async Task<Result> DropCourseAsync(long studentId, long taskId)
    {
        var record = await _selectionRepo.GetByStudentAndTaskAsync(studentId, taskId);
        if (record == null)
            return Result.Failure("未选择该课程", 40401);

        try
        {
            record.Drop();
            _selectionRepo.Update(record);

            var task = await _taskRepo.GetByIdAsync(taskId);
            if (task != null)
            {
                task.DecrementCount();
                _taskRepo.Update(task);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("退课成功: StudentId={StudentId}, TaskId={TaskId}", studentId, taskId);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, 40000);
        }
    }

    public async Task<IReadOnlyList<SelectionRecordDto>> GetMySelectionsAsync(long studentId, long semesterId)
    {
        var records = await _selectionRepo.GetByStudentAsync(studentId, semesterId);
        var dtos = new List<SelectionRecordDto>();

        foreach (var record in records)
        {
            var course = await _courseRepo.GetByIdAsync(record.CourseId);
            dtos.Add(new SelectionRecordDto
            {
                RecordId = record.RecordId,
                StudentId = record.StudentId,
                TaskId = record.TaskId,
                CourseId = record.CourseId,
                CourseName = course?.CourseName ?? "",
                RoundId = record.RoundId,
                SemesterId = record.SemesterId,
                Status = record.Status,
                SelectedAt = record.SelectedAt,
                DroppedAt = record.DroppedAt
            });
        }

        return dtos;
    }

    public async Task<IReadOnlyList<SelectionRecordDto>> GetTaskSelectionsAsync(long taskId)
    {
        var records = await _selectionRepo.GetByTaskAsync(taskId);
        var dtos = new List<SelectionRecordDto>();

        foreach (var record in records)
        {
            var course = await _courseRepo.GetByIdAsync(record.CourseId);
            dtos.Add(new SelectionRecordDto
            {
                RecordId = record.RecordId,
                StudentId = record.StudentId,
                TaskId = record.TaskId,
                CourseId = record.CourseId,
                CourseName = course?.CourseName ?? "",
                RoundId = record.RoundId,
                SemesterId = record.SemesterId,
                Status = record.Status,
                SelectedAt = record.SelectedAt,
                DroppedAt = record.DroppedAt
            });
        }

        return dtos;
    }
}