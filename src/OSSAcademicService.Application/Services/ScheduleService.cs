using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 排课服务实现
/// </summary>
public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepo;
    private readonly ITeachingTaskRepository _taskRepo;
    private readonly ICourseRepository _courseRepo;
    private readonly IClassroomRepository _classroomRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ScheduleService(
        IScheduleRepository scheduleRepo,
        ITeachingTaskRepository taskRepo,
        ICourseRepository courseRepo,
        IClassroomRepository classroomRepo,
        IUnitOfWork unitOfWork)
    {
        _scheduleRepo = scheduleRepo;
        _taskRepo = taskRepo;
        _courseRepo = courseRepo;
        _classroomRepo = classroomRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ScheduleItemDto>> GetScheduleBySemesterAsync(long semesterId)
    {
        var items = await _scheduleRepo.GetBySemesterAsync(semesterId);
        return await MapToDtos(items);
    }

    public async Task<IReadOnlyList<ScheduleItemDto>> GetScheduleByTeacherAsync(long teacherId, long semesterId)
    {
        var items = await _scheduleRepo.GetByTeacherAsync(teacherId, semesterId);
        return await MapToDtos(items);
    }

    public async Task<IReadOnlyList<ScheduleItemDto>> GetScheduleByStudentAsync(long studentId, long semesterId)
    {
        var items = await _scheduleRepo.GetBySemesterAsync(semesterId);
        return await MapToDtos(items);
    }

    public async Task<OSSAcademicService.Application.Common.Result<long>> AddScheduleItemAsync(AddScheduleItemDto dto)
    {
        var task = await _taskRepo.GetByIdAsync(dto.TaskId);
        if (task == null)
            return OSSAcademicService.Application.Common.Result<long>.Failure("教学任务不存在", 40401);

        var classroom = await _classroomRepo.GetByIdAsync(dto.ClassroomId);
        if (classroom == null)
            return OSSAcademicService.Application.Common.Result<long>.Failure("教室不存在", 40401);

        var existingItems = await _scheduleRepo.GetByTeacherAsync(dto.TeacherId, dto.SemesterId);
        var newItem = ScheduleItem.Create(dto.TaskId, dto.CourseId, dto.TeacherId,
            dto.ClassroomId, dto.SemesterId, dto.DayOfWeek, dto.StartPeriod, dto.EndPeriod,
            dto.StartWeek, dto.EndWeek, dto.WeekType);

        var hasTeacherConflict = existingItems.Any(e => newItem.OverlapsWith(e));
        if (hasTeacherConflict)
            return OSSAcademicService.Application.Common.Result<long>.Failure("教师时间冲突", 40901);

        var classroomItems = await _scheduleRepo.GetByClassroomAsync(dto.ClassroomId, dto.SemesterId);
        var hasClassroomConflict = classroomItems.Any(e => newItem.OverlapsWith(e));
        if (hasClassroomConflict)
            return OSSAcademicService.Application.Common.Result<long>.Failure("教室时间冲突", 40902);

        _scheduleRepo.Add(newItem);
        await _unitOfWork.SaveChangesAsync();

        return OSSAcademicService.Application.Common.Result<long>.Success(newItem.ItemId);
    }

    public async Task<OSSAcademicService.Application.Common.Result> RemoveScheduleItemAsync(long itemId)
    {
        var item = await _scheduleRepo.GetByIdAsync(itemId);
        if (item == null)
            return OSSAcademicService.Application.Common.Result.Failure("排课条目不存在", 40401);

        if (item.IsLocked)
            return OSSAcademicService.Application.Common.Result.Failure("排课已锁定，不可删除", 40301);

        _scheduleRepo.Remove(item);
        await _unitOfWork.SaveChangesAsync();

        return OSSAcademicService.Application.Common.Result.Success();
    }

    private async Task<IReadOnlyList<ScheduleItemDto>> MapToDtos(IReadOnlyList<ScheduleItem> items)
    {
        var dtos = new List<ScheduleItemDto>();
        foreach (var item in items)
        {
            var course = await _courseRepo.GetByIdAsync(item.CourseId);
            var classroom = await _classroomRepo.GetByIdAsync(item.ClassroomId);
            dtos.Add(new ScheduleItemDto
            {
                ItemId = item.ItemId,
                TaskId = item.TaskId,
                CourseId = item.CourseId,
                CourseName = course?.CourseName ?? "",
                TeacherId = item.TeacherId,
                ClassroomId = item.ClassroomId,
                ClassroomName = classroom?.RoomName ?? "",
                SemesterId = item.SemesterId,
                DayOfWeek = item.DayOfWeek,
                StartPeriod = item.StartPeriod,
                EndPeriod = item.EndPeriod,
                StartWeek = item.StartWeek,
                EndWeek = item.EndWeek,
                WeekType = item.WeekType
            });
        }
        return dtos;
    }
}