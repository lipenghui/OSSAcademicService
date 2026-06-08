using Microsoft.Extensions.Logging;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Enums;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 教室服务实现
/// </summary>
public class ClassroomService : IClassroomService
{
    private readonly IClassroomRepository _classroomRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ClassroomService> _logger;

    public ClassroomService(
        IClassroomRepository classroomRepo,
        IUnitOfWork unitOfWork,
        ILogger<ClassroomService> logger)
    {
        _classroomRepo = classroomRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResult<ClassroomDto>> GetClassroomsPagedAsync(
        long? buildingId, string? roomType, int? minCapacity, int page, int pageSize)
    {
        var (items, totalCount) = await _classroomRepo.GetPagedAsync(buildingId, roomType, minCapacity, page, pageSize);

        var dtos = new List<ClassroomDto>();
        foreach (var classroom in items)
        {
            var building = await _classroomRepo.GetBuildingByIdAsync(classroom.BuildingId);
            dtos.Add(new ClassroomDto
            {
                ClassroomId = classroom.ClassroomId,
                BuildingId = classroom.BuildingId,
                BuildingName = building?.BuildingName ?? "",
                RoomNo = classroom.RoomNo,
                RoomName = classroom.RoomName,
                RoomType = classroom.RoomType,
                Capacity = classroom.Capacity,
                Status = classroom.Status
            });
        }

        return new PagedResult<ClassroomDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<Result<ClassroomDetailDto?>> GetClassroomAsync(long classroomId)
    {
        var classroom = await _classroomRepo.GetByIdAsync(classroomId);
        if (classroom == null)
            return Result<ClassroomDetailDto?>.Failure("教室不存在", 40401);

        var building = await _classroomRepo.GetBuildingByIdAsync(classroom.BuildingId);

        return Result<ClassroomDetailDto?>.Success(new ClassroomDetailDto
        {
            ClassroomId = classroom.ClassroomId,
            BuildingId = classroom.BuildingId,
            BuildingName = building?.BuildingName ?? "",
            RoomNo = classroom.RoomNo,
            RoomName = classroom.RoomName,
            RoomType = classroom.RoomType,
            Capacity = classroom.Capacity,
            FloorNo = classroom.FloorNo,
            HasMultimedia = classroom.HasMultimedia,
            HasComputer = classroom.HasComputer,
            Equipment = classroom.Equipment,
            Status = classroom.Status
        });
    }

    public async Task<IReadOnlyList<ClassroomDto>> GetAvailableClassroomsAsync(
        long semesterId, int dayOfWeek, int period, int startWeek, int minCapacity)
    {
        var classrooms = await _classroomRepo.GetAvailableAsync(semesterId, dayOfWeek, period, startWeek, minCapacity);

        var dtos = new List<ClassroomDto>();
        foreach (var classroom in classrooms)
        {
            var building = await _classroomRepo.GetBuildingByIdAsync(classroom.BuildingId);
            dtos.Add(new ClassroomDto
            {
                ClassroomId = classroom.ClassroomId,
                BuildingId = classroom.BuildingId,
                BuildingName = building?.BuildingName ?? "",
                RoomNo = classroom.RoomNo,
                RoomName = classroom.RoomName,
                RoomType = classroom.RoomType,
                Capacity = classroom.Capacity,
                Status = classroom.Status
            });
        }

        return dtos;
    }

    public async Task<Result<long>> BookClassroomAsync(
        long classroomId, long applicantId, string purpose,
        DateOnly bookingDate, int startPeriod, int endPeriod)
    {
        var classroom = await _classroomRepo.GetByIdAsync(classroomId);
        if (classroom == null)
            return Result<long>.Failure("教室不存在", 40401);

        var existingBookings = await _classroomRepo.GetBookingsByClassroomAsync(classroomId, bookingDate);
        var hasConflict = existingBookings.Any(b =>
            b.ApplyStatus == ApplyStatus.Approved &&
            b.StartPeriod <= endPeriod && b.EndPeriod >= startPeriod);

        if (hasConflict)
            return Result<long>.Failure("该时间段教室已被占用", 40901);

        var booking = ClassroomBooking.Create(classroomId, applicantId, bookingDate, startPeriod, endPeriod, purpose);
        _classroomRepo.AddBooking(booking);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("教室借用申请已提交: BookingId={BookingId}", booking.BookingId);
        return Result<long>.Success(booking.BookingId);
    }
}