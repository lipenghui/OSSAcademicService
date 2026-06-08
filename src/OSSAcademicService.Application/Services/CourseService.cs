using Microsoft.Extensions.Logging;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Exceptions;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 课程管理服务实现
/// </summary>
public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepo;
    private readonly IBaseDataRepository _baseDataRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CourseService> _logger;

    public CourseService(
        ICourseRepository courseRepo,
        IBaseDataRepository baseDataRepo,
        IUnitOfWork unitOfWork,
        ILogger<CourseService> logger)
    {
        _courseRepo = courseRepo;
        _baseDataRepo = baseDataRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CourseDetailDto?>> GetCourseAsync(long courseId)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
            return Result<CourseDetailDto?>.Failure("课程不存在", 40401);

        var college = await _baseDataRepo.GetCollegeByIdAsync(course.CollegeId);
        return Result<CourseDetailDto?>.Success(new CourseDetailDto
        {
            CourseId = course.CourseId,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            CourseNameEn = course.CourseNameEn,
            Credit = course.Credit,
            TotalHours = course.TotalHours,
            TeachingHours = course.TeachingHours,
            LabHours = course.LabHours,
            PracticeHours = course.PracticeHours,
            TeachingMode = course.TeachingMode,
            CourseType = course.CourseType,
            CollegeId = course.CollegeId,
            OutlineUrl = course.OutlineUrl,
            MaxCapacity = course.MaxCapacity,
            AssessMethod = course.AssessMethod,
            IsActive = course.IsActive
        });
    }

    public async Task<PagedResult<CourseListDto>> GetCoursesPagedAsync(
        string? keyword, string? courseType, long? collegeId, int page, int pageSize)
    {
        var (items, totalCount) = await _courseRepo.GetPagedAsync(keyword, courseType, collegeId, page, pageSize);

        var dtos = new List<CourseListDto>();
        foreach (var course in items)
        {
            var college = await _baseDataRepo.GetCollegeByIdAsync(course.CollegeId);
            dtos.Add(new CourseListDto
            {
                CourseId = course.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.CourseName,
                Credit = course.Credit,
                CourseType = course.CourseType,
                CollegeId = course.CollegeId
            });
        }

        return new PagedResult<CourseListDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<Result<long>> CreateCourseAsync(CreateCourseDto dto)
    {
        var existing = await _courseRepo.GetByCodeAsync(dto.CourseCode);
        if (existing != null)
            return Result<long>.Failure("课程编号已存在", 40901);

        try
        {
            var course = Course.Create(
                dto.CourseCode, dto.CourseName, dto.Credit, dto.TotalHours,
                dto.CourseType, dto.CollegeId, dto.TeachingMode);

            course.Update(dto.CourseName, dto.Credit, dto.TotalHours,
                dto.TeachingHours, dto.LabHours, dto.PracticeHours, null);

            _courseRepo.Add(course);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("课程已创建: CourseId={CourseId}, Code={Code}", course.CourseId, course.CourseCode);
            return Result<long>.Success(course.CourseId);
        }
        catch (DomainException ex)
        {
            return Result<long>.Failure(ex.Message, 40000);
        }
    }

    public async Task<Result> UpdateCourseAsync(long courseId, UpdateCourseDto dto)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure("课程不存在", 40401);

        course.Update(dto.CourseName, dto.Credit, dto.TotalHours,
            dto.TeachingHours, dto.LabHours, dto.PracticeHours, dto.OutlineUrl);

        _courseRepo.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteCourseAsync(long courseId)
    {
        var course = await _courseRepo.GetByIdAsync(courseId);
        if (course == null)
            return Result.Failure("课程不存在", 40401);

        course.SoftDelete();
        _courseRepo.Update(course);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}