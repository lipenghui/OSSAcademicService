using Microsoft.Extensions.Logging;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 考试服务实现
/// </summary>
public class ExamService : IExamService
{
    private readonly IExamRepository _examRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly IClassroomRepository _classroomRepo;
    private readonly ITeachingTaskRepository _taskRepo;
    private readonly ISelectionRepository _selectionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ExamService> _logger;

    public ExamService(
        IExamRepository examRepo,
        ICourseRepository courseRepo,
        IClassroomRepository classroomRepo,
        ITeachingTaskRepository taskRepo,
        ISelectionRepository selectionRepo,
        IUnitOfWork unitOfWork,
        ILogger<ExamService> logger)
    {
        _examRepo = examRepo;
        _courseRepo = courseRepo;
        _classroomRepo = classroomRepo;
        _taskRepo = taskRepo;
        _selectionRepo = selectionRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ExamArrangementDto>> GetExamsAsync(long semesterId, string? examType)
    {
        var arrangements = await _examRepo.GetBySemesterAsync(semesterId, examType);
        var dtos = new List<ExamArrangementDto>();

        foreach (var arr in arrangements)
        {
            var course = await _courseRepo.GetByIdAsync(arr.CourseId);
            var classroom = await _classroomRepo.GetByIdAsync(arr.ClassroomId);

            dtos.Add(new ExamArrangementDto
            {
                ArrangementId = arr.ArrangementId,
                CourseId = arr.CourseId,
                CourseName = course?.CourseName ?? "",
                TaskId = arr.TaskId,
                SemesterId = arr.SemesterId,
                ExamType = arr.ExamType,
                ExamDate = arr.ExamDate,
                StartTime = arr.StartTime,
                EndTime = arr.EndTime,
                ClassroomId = arr.ClassroomId,
                ClassroomName = classroom?.RoomName ?? "",
                ExamForm = arr.ExamForm,
                Status = arr.Status
            });
        }

        return dtos;
    }

    public async Task<Result<long>> CreateExamAsync(CreateExamDto dto)
    {
        var arrangement = ExamArrangement.Create(
            dto.CourseId, dto.TaskId, dto.SemesterId, dto.ExamType,
            dto.ExamDate, dto.StartTime, dto.EndTime, dto.ClassroomId, dto.ExamForm);

        _examRepo.AddArrangement(arrangement);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("考试安排已创建: ArrangementId={ArrangementId}", arrangement.ArrangementId);
        return Result<long>.Success(arrangement.ArrangementId);
    }

    public async Task<IReadOnlyList<ExamSeatDto>> GetSeatsAsync(long arrangementId)
    {
        var seats = await _examRepo.GetSeatsByArrangementAsync(arrangementId);
        return seats.Select(s => new ExamSeatDto
        {
            Id = s.Id,
            ArrangementId = s.ArrangementId,
            StudentId = s.StudentId,
            SeatNo = s.SeatNo,
            ExamNo = s.ExamNo,
            CheckInTime = s.CheckInTime
        }).ToList();
    }

    public async Task<Result> AssignSeatsAsync(long arrangementId)
    {
        var arrangement = await _examRepo.GetByIdAsync(arrangementId);
        if (arrangement == null)
            return Result.Failure("考试安排不存在", 40401);

        var selections = await _selectionRepo.GetByTaskAsync(arrangement.TaskId);
        var activeSelections = selections.Where(s => s.Status == "已选").ToList();

        var classroom = await _classroomRepo.GetByIdAsync(arrangement.ClassroomId);
        if (classroom == null)
            return Result.Failure("教室不存在", 40401);

        var seatNo = 1;
        foreach (var selection in activeSelections)
        {
            if (seatNo <= classroom.Capacity)
            {
                var seat = new ExamSeat(arrangementId, selection.StudentId, seatNo);
                _examRepo.AddSeat(seat);
                seatNo++;
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> AssignInvigilatorsAsync(long arrangementId, IReadOnlyList<long> teacherIds)
    {
        var arrangement = await _examRepo.GetByIdAsync(arrangementId);
        if (arrangement == null)
            return Result.Failure("考试安排不存在", 40401);

        var existingInvigilations = await _examRepo.GetInvigilationsByArrangementAsync(arrangementId);
        if (existingInvigilations.Any())
            return Result.Failure("已存在监考安排", 40901);

        for (var i = 0; i < teacherIds.Count; i++)
        {
            var role = i == 0 ? "主监考" : "副监考";
            var invigilation = new Invigilation(arrangementId, teacherIds[i], role);
            _examRepo.AddInvigilation(invigilation);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<long>> ApplyDeferredExamAsync(long studentId, long arrangementId, string reason, string? proofUrl)
    {
        var existing = await _examRepo.GetDeferredByStudentAsync(studentId);
        if (existing.Any(d => d.ArrangementId == arrangementId))
            return Result<long>.Failure("已申请缓考", 40901);

        var deferred = DeferredExam.Create(studentId, arrangementId, reason, proofUrl);
        _examRepo.AddDeferred(deferred);
        await _unitOfWork.SaveChangesAsync();

        return Result<long>.Success(deferred.DeferredId);
    }
}