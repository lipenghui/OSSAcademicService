using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 选课管理控制器
/// </summary>
[ApiController]
[Route("api/academic/selections")]
public class SelectionController : ControllerBase
{
    private readonly ISelectionService _selectionService;

    public SelectionController(ISelectionService selectionService) => _selectionService = selectionService;

    [HttpGet("rounds")]
    public async Task<IActionResult> GetRounds([FromQuery] long semesterId)
    {
        var result = await _selectionService.GetRoundsAsync(semesterId);
        return Ok(ApiResponse<IReadOnlyList<SelectionRoundDto>>.Ok(result));
    }

    [HttpPost("rounds")]
    public async Task<IActionResult> CreateRound([FromBody] CreateSelectionRoundDto dto)
    {
        var result = await _selectionService.CreateRoundAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpPost("rounds/{roundId}/open")]
    public async Task<IActionResult> OpenRound(long roundId)
    {
        var result = await _selectionService.OpenRoundAsync(roundId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost("rounds/{roundId}/close")]
    public async Task<IActionResult> CloseRound(long roundId)
    {
        var result = await _selectionService.CloseRoundAsync(roundId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost]
    public async Task<IActionResult> SelectCourse([FromBody] SelectCourseRequest request)
    {
        var studentId = GetCurrentStudentId();
        var result = await _selectionService.SelectCourseAsync(studentId, request.TaskId, request.RoundId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DropCourse(long taskId)
    {
        var studentId = GetCurrentStudentId();
        var result = await _selectionService.DropCourseAsync(studentId, taskId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMySelections([FromQuery] long semesterId)
    {
        var studentId = GetCurrentStudentId();
        var result = await _selectionService.GetMySelectionsAsync(studentId, semesterId);
        return Ok(ApiResponse<IReadOnlyList<SelectionRecordDto>>.Ok(result));
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetTaskSelections(long taskId)
    {
        var result = await _selectionService.GetTaskSelectionsAsync(taskId);
        return Ok(ApiResponse<IReadOnlyList<SelectionRecordDto>>.Ok(result));
    }

    private long GetCurrentStudentId() =>
        long.Parse(User.FindFirst("student_id")?.Value ?? "0");
}

public record SelectCourseRequest(long TaskId, long RoundId);