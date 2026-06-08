using Microsoft.AspNetCore.Mvc;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;

namespace OSSAcademicService.API.Controllers;

/// <summary>
/// 课程管理控制器
/// </summary>
[ApiController]
[Route("api/academic/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService) => _courseService = courseService;

    [HttpGet]
    public async Task<IActionResult> GetCourses(
        [FromQuery] string? keyword, [FromQuery] string? courseType,
        [FromQuery] long? collegeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _courseService.GetCoursesPagedAsync(keyword, courseType, collegeId, page, pageSize);
        return Ok(ApiResponse<PagedResult<CourseListDto>>.Ok(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(long id)
    {
        var result = await _courseService.GetCourseAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResponse<object>.Fail(40401, result.ErrorMessage!));
        return Ok(ApiResponse<CourseDetailDto>.Ok(result.Data!));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        var result = await _courseService.CreateCourseAsync(dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<long>.Ok(result.Data));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(long id, [FromBody] UpdateCourseDto dto)
    {
        var result = await _courseService.UpdateCourseAsync(id, dto);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(long id)
    {
        var result = await _courseService.DeleteCourseAsync(id);
        if (!result.IsSuccess)
            return BadRequest(ApiResponse<object>.Fail(40000, result.ErrorMessage!));
        return Ok(ApiResponse<object>.Ok(null!));
    }
}