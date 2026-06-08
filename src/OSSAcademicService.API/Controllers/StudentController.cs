using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 学生管理控制器
/// </summary>
[ApiController]
[Route("api/academic/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService) => _studentService = studentService;

    [HttpGet]
    public async Task<IActionResult> GetStudents(
        [FromQuery] long? collegeId, [FromQuery] long? majorId,
        [FromQuery] long? classId, [FromQuery] string? status,
        [FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _studentService.GetStudentsPagedAsync(collegeId, majorId, classId, status, keyword, page, pageSize);
        return Ok(ApiResponse<PagedResult<StudentListDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(long id)
    {
        var result = await _studentService.GetProfileAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<object>.Fail(40401, result.ErrorMessage!));
        return Ok(ApiResponse<StudentProfileDto>.Ok(result.Data!));
    }

    [HttpGet("no/{studentNo}")]
    public async Task<IActionResult> GetStudentByNo(string studentNo)
    {
        var result = await _studentService.GetProfileByNoAsync(studentNo);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<object>.Fail(40401, result.ErrorMessage!));
        return Ok(ApiResponse<StudentProfileDto>.Ok(result.Data!));
    }

    [HttpPost("{id}/status-change")]
    public async Task<IActionResult> SubmitStatusChange(long id, [FromBody] StatusChangeRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _studentService.SubmitStatusChangeAsync(id, request.ChangeType, request.Reason, userId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpGet("{id}/status-changes")]
    public async Task<IActionResult> GetStatusChanges(long id)
    {
        var result = await _studentService.GetStatusChangesAsync(id);
        return Ok(ApiResponse<IReadOnlyList<StatusChangeDto>>.Ok(result));
    }

    [HttpPost("{id}/graduation-audit")]
    public async Task<IActionResult> InitiateGraduationAudit(long id, [FromQuery] long semesterId)
    {
        var result = await _studentService.InitiateGraduationAuditAsync(id, semesterId);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpGet("{id}/graduation-audit")]
    public async Task<IActionResult> GetGraduationAudit(long id, [FromQuery] long semesterId)
    {
        var result = await _studentService.GetGraduationAuditAsync(id, semesterId);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<object>.Fail(40401, result.ErrorMessage!));
        return Ok(ApiResponse<GraduationAuditDto>.Ok(result.Data!));
    }

    private long GetCurrentUserId() =>
        long.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
}

public record StatusChangeRequest(string ChangeType, string Reason);