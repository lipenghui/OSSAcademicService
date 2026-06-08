using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 基础数据控制器
/// </summary>
[ApiController]
[Route("api/academic/base-data")]
public class BaseDataController : ControllerBase
{
    private readonly IBaseDataService _baseDataService;

    public BaseDataController(IBaseDataService baseDataService) => _baseDataService = baseDataService;

    [HttpGet("semesters")]
    public async Task<IActionResult> GetSemesters()
    {
        var result = await _baseDataService.GetSemestersAsync();
        return Ok(ApiResponse<IReadOnlyList<SemesterDto>>.Ok(result));
    }

    [HttpGet("semesters/current")]
    public async Task<IActionResult> GetCurrentSemester()
    {
        var result = await _baseDataService.GetCurrentSemesterAsync();
        if (result == null)
            return NotFound(ApiResponse<object>.Fail(40401, "当前学期未设置"));
        return Ok(ApiResponse<SemesterDto>.Ok(result));
    }

    [HttpGet("colleges")]
    public async Task<IActionResult> GetColleges()
    {
        var result = await _baseDataService.GetCollegesAsync();
        return Ok(ApiResponse<IReadOnlyList<CollegeDto>>.Ok(result));
    }

    [HttpGet("colleges/{collegeId}/majors")]
    public async Task<IActionResult> GetMajors(long collegeId)
    {
        var result = await _baseDataService.GetMajorsAsync(collegeId);
        return Ok(ApiResponse<IReadOnlyList<MajorDto>>.Ok(result));
    }

    [HttpGet("majors/{majorId}/classes")]
    public async Task<IActionResult> GetClasses(long majorId)
    {
        var result = await _baseDataService.GetClassesAsync(majorId);
        return Ok(ApiResponse<IReadOnlyList<ClassDto>>.Ok(result));
    }
}