using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 排课管理控制器
/// </summary>
[ApiController]
[Route("api/academic/schedules")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService) => _scheduleService = scheduleService;

    [HttpGet("semester/{semesterId}")]
    public async Task<IActionResult> GetScheduleBySemester(long semesterId)
    {
        var result = await _scheduleService.GetScheduleBySemesterAsync(semesterId);
        return Ok(ApiResponse<IReadOnlyList<ScheduleItemDto>>.Ok(result));
    }

    [HttpGet("teacher/{teacherId}")]
    public async Task<IActionResult> GetScheduleByTeacher(long teacherId, [FromQuery] long semesterId)
    {
        var result = await _scheduleService.GetScheduleByTeacherAsync(teacherId, semesterId);
        return Ok(ApiResponse<IReadOnlyList<ScheduleItemDto>>.Ok(result));
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetScheduleByStudent(long studentId, [FromQuery] long semesterId)
    {
        var result = await _scheduleService.GetScheduleByStudentAsync(studentId, semesterId);
        return Ok(ApiResponse<IReadOnlyList<ScheduleItemDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> AddScheduleItem([FromBody] AddScheduleItemDto dto)
    {
        var result = await _scheduleService.AddScheduleItemAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(result.ErrorCode, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> RemoveScheduleItem(long itemId)
    {
        var result = await _scheduleService.RemoveScheduleItemAsync(itemId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(result.ErrorCode, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }
}