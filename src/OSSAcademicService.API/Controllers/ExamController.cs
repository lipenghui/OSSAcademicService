using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 考试管理控制器
/// </summary>
[ApiController]
[Route("api/academic/exams")]
public class ExamController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamController(IExamService examService) => _examService = examService;

    [HttpGet("semester/{semesterId}")]
    public async Task<IActionResult> GetExams(long semesterId, [FromQuery] string? examType)
    {
        var result = await _examService.GetExamsAsync(semesterId, examType);
        return Ok(ApiResponse<IReadOnlyList<ExamArrangementDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateExam([FromBody] CreateExamDto dto)
    {
        var result = await _examService.CreateExamAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpGet("{id}/seats")]
    public async Task<IActionResult> GetSeats(long id)
    {
        var result = await _examService.GetSeatsAsync(id);
        return Ok(ApiResponse<IReadOnlyList<ExamSeatDto>>.Ok(result));
    }

    [HttpPost("{id}/assign-seats")]
    public async Task<IActionResult> AssignSeats(long id)
    {
        var result = await _examService.AssignSeatsAsync(id);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost("{id}/assign-invigilators")]
    public async Task<IActionResult> AssignInvigilators(long id, [FromBody] AssignInvigilatorsRequest request)
    {
        var result = await _examService.AssignInvigilatorsAsync(id, request.TeacherIds);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpPost("deferred")]
    public async Task<IActionResult> ApplyDeferredExam([FromBody] DeferredExamRequest request)
    {
        var studentId = GetCurrentStudentId();
        var result = await _examService.ApplyDeferredExamAsync(studentId, request.ArrangementId, request.Reason, request.ProofUrl);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    private long GetCurrentStudentId() =>
        long.Parse(User.FindFirst("student_id")?.Value ?? "0");
}

public record AssignInvigilatorsRequest(IReadOnlyList<long> TeacherIds);
public record DeferredExamRequest(long ArrangementId, string Reason, string? ProofUrl);