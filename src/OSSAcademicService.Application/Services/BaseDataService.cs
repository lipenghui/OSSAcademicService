using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 基础数据服务实现
/// </summary>
public class BaseDataService : IBaseDataService
{
    private readonly ISemesterRepository _semesterRepo;
    private readonly IBaseDataRepository _baseDataRepo;

    public BaseDataService(ISemesterRepository semesterRepo, IBaseDataRepository baseDataRepo)
    {
        _semesterRepo = semesterRepo;
        _baseDataRepo = baseDataRepo;
    }

    public async Task<IReadOnlyList<SemesterDto>> GetSemestersAsync()
    {
        var semesters = await _semesterRepo.GetAllAsync();
        return semesters.Select(s => new SemesterDto
        {
            SemesterId = s.SemesterId,
            SemesterName = s.SemesterName,
            AcademicYear = s.AcademicYear,
            Term = s.Term,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsCurrent = s.IsCurrent
        }).ToList();
    }

    public async Task<SemesterDto?> GetCurrentSemesterAsync()
    {
        var semester = await _semesterRepo.GetCurrentAsync();
        if (semester == null) return null;

        return new SemesterDto
        {
            SemesterId = semester.SemesterId,
            SemesterName = semester.SemesterName,
            AcademicYear = semester.AcademicYear,
            Term = semester.Term,
            StartDate = semester.StartDate,
            EndDate = semester.EndDate,
            IsCurrent = semester.IsCurrent
        };
    }

    public async Task<IReadOnlyList<CollegeDto>> GetCollegesAsync()
    {
        var colleges = await _baseDataRepo.GetAllCollegesAsync();
        return colleges.Select(c => new CollegeDto
        {
            CollegeId = c.CollegeId,
            CollegeCode = c.CollegeCode,
            CollegeName = c.CollegeName,
            ShortName = c.ShortName
        }).ToList();
    }

    public async Task<IReadOnlyList<MajorDto>> GetMajorsAsync(long collegeId)
    {
        var majors = await _baseDataRepo.GetMajorsByCollegeAsync(collegeId);
        return majors.Select(m => new MajorDto
        {
            MajorId = m.MajorId,
            MajorCode = m.MajorCode,
            MajorName = m.MajorName,
            CollegeId = m.CollegeId,
            EduLevel = m.EduLevel,
            LengthOfSchool = m.LengthOfSchool
        }).ToList();
    }

    public async Task<IReadOnlyList<ClassDto>> GetClassesAsync(long majorId)
    {
        var classes = await _baseDataRepo.GetClassesByMajorAsync(majorId);
        return classes.Select(c => new ClassDto
        {
            ClassId = c.ClassId,
            ClassCode = c.ClassCode,
            ClassName = c.ClassName,
            MajorId = c.MajorId,
            GradeYear = c.GradeYear,
            CurrentCount = c.CurrentCount
        }).ToList();
    }
}