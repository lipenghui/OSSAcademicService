using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 成绩管理控制器
/// </summary>
[ApiController]
[Route("api/academic/scores")]
public class ScoreController : ControllerBase
{
    private readonly IScoreService _scoreService;

    public ScoreController(IScoreService scoreService) => _scoreService = scoreService;

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetTaskScores(long taskId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _scoreService.GetTaskScoresAsync(taskId, page, pageSize);
        return Ok(ApiResponse<PagedResult<ScoreRecordDto>>.Ok(result));
    }

    [HttpPost("task/{taskId}")]
    public async Task<IActionResult> EnterScore(long taskId, [FromBody] EnterScoreRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _scoreService.EnterScoreAsync(request.StudentId, taskId, request.EnterScoreDto, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost("task/{taskId}/batch")]
    public async Task<IActionResult> BatchEnterScores(long taskId, [FromBody] BatchEnterScoresRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _scoreService.BatchEnterScoresAsync(taskId, request.Entries, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost("task/{taskId}/publish")]
    public async Task<IActionResult> PublishScores(long taskId)
    {
        var userId = GetCurrentUserId();
        var result = await _scoreService.PublishScoresAsync(taskId, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetTranscript(long studentId, [FromQuery] long? semesterId)
    {
        var result = await _scoreService.GetTranscriptAsync(studentId, semesterId);
        return Ok(ApiResponse<StudentTranscriptDto>.Ok(result));
    }

    [HttpGet("student/{studentId}/gpa")]
    public async Task<IActionResult> GetGpa(long studentId, [FromQuery] long? semesterId)
    {
        var result = await _scoreService.GetGpaAsync(studentId, semesterId);
        return Ok(ApiResponse<GpaSummaryDto>.Ok(result));
    }

    [HttpGet("analysis/task/{taskId}")]
    public async Task<IActionResult> GetAnalysis(long taskId)
    {
        var result = await _scoreService.GetAnalysisAsync(taskId);
        return Ok(ApiResponse<ScoreAnalysisDto>.Ok(result));
    }

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
}

public record EnterScoreRequest(long StudentId, EnterScoreDto EnterScoreDto);
public record BatchEnterScoresRequest(IReadOnlyList<BatchScoreEntryDto> Entries);