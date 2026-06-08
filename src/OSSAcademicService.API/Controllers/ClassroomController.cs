using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 教室管理控制器
/// </summary>
[ApiController]
[Route("api/academic/classrooms")]
public class ClassroomController : ControllerBase
{
    private readonly IClassroomService _classroomService;

    public ClassroomController(IClassroomService classroomService) => _classroomService = classroomService;

    [HttpGet]
    public async Task<IActionResult> GetClassrooms(
        [FromQuery] long? buildingId, [FromQuery] string? roomType,
        [FromQuery] int? minCapacity, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _classroomService.GetClassroomsPagedAsync(buildingId, roomType, minCapacity, page, pageSize);
        return Ok(ApiResponse<PagedResult<ClassroomDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClassroom(long id)
    {
        var result = await _classroomService.GetClassroomAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<object>.Fail(40401, result.ErrorMessage!));
        return Ok(ApiResponse<ClassroomDetailDto>.Ok(result.Data!));
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableClassrooms(
        [FromQuery] long semesterId, [FromQuery] int dayOfWeek,
        [FromQuery] int period, [FromQuery] int startWeek, [FromQuery] int minCapacity)
    {
        var result = await _classroomService.GetAvailableClassroomsAsync(semesterId, dayOfWeek, period, startWeek, minCapacity);
        return Ok(ApiResponse<IReadOnlyList<ClassroomDto>>.Ok(result));
    }

    [HttpPost("{id}/book")]
    public async Task<IActionResult> BookClassroom(long id, [FromBody] BookClassroomRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _classroomService.BookClassroomAsync(id, userId, request.Purpose, request.BookingDate, request.StartPeriod, request.EndPeriod);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
}

public record BookClassroomRequest(string Purpose, DateOnly BookingDate, int StartPeriod, int EndPeriod);