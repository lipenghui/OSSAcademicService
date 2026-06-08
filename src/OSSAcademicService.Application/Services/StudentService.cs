using Microsoft.Extensions.Logging;
using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Enums;
using OSSAcademicService.Domain.Exceptions;

namespace OSSAcademicService.Application.Services;

/// <summary>
/// 学生管理服务实现
/// </summary>
public class StudentService : IStudentService
{
    private readonly IStudentProfileRepository _studentRepo;
    private readonly IStatusChangeRepository _statusChangeRepo;
    private readonly IGraduationAuditRepository _graduationRepo;
    private readonly IBaseDataRepository _baseDataRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StudentService> _logger;

    public StudentService(
        IStudentProfileRepository studentRepo,
        IStatusChangeRepository statusChangeRepo,
        IGraduationAuditRepository graduationRepo,
        IBaseDataRepository baseDataRepo,
        IUnitOfWork unitOfWork,
        ILogger<StudentService> logger)
    {
        _studentRepo = studentRepo;
        _statusChangeRepo = statusChangeRepo;
        _graduationRepo = graduationRepo;
        _baseDataRepo = baseDataRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<StudentProfileDto?>> GetProfileAsync(long studentId)
    {
        var profile = await _studentRepo.GetByIdAsync(studentId);
        if (profile == null)
            return Result<StudentProfileDto?>.Failure("学生不存在", 40401);

        return Result<StudentProfileDto?>.Success(await MapToDto(profile));
    }

    public async Task<Result<StudentProfileDto?>> GetProfileByNoAsync(string studentNo)
    {
        var profile = await _studentRepo.GetByStudentNoAsync(studentNo);
        if (profile == null)
            return Result<StudentProfileDto?>.Failure("学生不存在", 40401);

        return Result<StudentProfileDto?>.Success(await MapToDto(profile));
    }

    public async Task<PagedResult<StudentListDto>> GetStudentsPagedAsync(
        long? collegeId, long? majorId, long? classId,
        string? status, string? keyword, int page, int pageSize)
    {
        StudentStatus? statusEnum = string.IsNullOrEmpty(status) ? null : Enum.Parse<StudentStatus>(status);
        var (items, totalCount) = await _studentRepo.GetPagedAsync(collegeId, majorId, classId, statusEnum, keyword, page, pageSize);

        var dtos = new List<StudentListDto>();
        foreach (var item in items)
        {
            var college = await _baseDataRepo.GetCollegeByIdAsync(item.CollegeId);
            var major = await _baseDataRepo.GetMajorByIdAsync(item.MajorId);
            var @class = item.ClassId.HasValue ? await _baseDataRepo.GetClassByIdAsync(item.ClassId.Value) : null;

            dtos.Add(new StudentListDto
            {
                StudentId = item.StudentId,
                StudentNo = item.StudentNo,
                CollegeId = item.CollegeId,
                CollegeName = college?.CollegeName ?? "",
                MajorId = item.MajorId,
                MajorName = major?.MajorName ?? "",
                ClassId = item.ClassId,
                ClassName = @class?.ClassName,
                GradeYear = item.GradeYear,
                Status = item.Status.ToString()
            });
        }

        return new PagedResult<StudentListDto>(dtos, totalCount, page, pageSize);
    }

    public async Task<Result<long>> SubmitStatusChangeAsync(long studentId, string changeType, string reason, long appliedBy)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null)
            return Result<long>.Failure("学生不存在", 40401);

        try
        {
            var changeTypeEnum = Enum.Parse<ChangeType>(changeType);
            var change = StatusChange.Create(studentId, changeTypeEnum, reason, appliedBy);
            _statusChangeRepo.Add(change);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("学籍异动申请已提交: StudentId={StudentId}, ChangeType={ChangeType}", studentId, changeType);
            return Result<long>.Success(change.ChangeId);
        }
        catch (DomainException ex)
        {
            return Result<long>.Failure(ex.Message, 40000);
        }
    }

    public async Task<IReadOnlyList<StatusChangeDto>> GetStatusChangesAsync(long studentId)
    {
        var changes = await _statusChangeRepo.GetByStudentAsync(studentId);
        return changes.Select(c => new StatusChangeDto
        {
            ChangeId = c.ChangeId,
            StudentId = c.StudentId,
            ChangeType = c.ChangeType.ToString(),
            ChangeReason = c.ChangeReason,
            ApplyStatus = c.ApplyStatus.ToString(),
            AppliedBy = c.AppliedBy,
            AppliedAt = c.AppliedAt,
            EffectiveDate = c.EffectiveDate,
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    public async Task<Result<long>> InitiateGraduationAuditAsync(long studentId, long semesterId)
    {
        var student = await _studentRepo.GetByIdAsync(studentId);
        if (student == null)
            return Result<long>.Failure("学生不存在", 40401);

        var existing = await _graduationRepo.GetByStudentAndSemesterAsync(studentId, semesterId);
        if (existing != null)
            return Result<long>.Failure("该学期已存在毕业审核记录", 40901);

        var audit = GraduationAudit.Create(studentId, semesterId, 160);
        _graduationRepo.Add(audit);
        await _unitOfWork.SaveChangesAsync();

        return Result<long>.Success(audit.AuditId);
    }

    public async Task<Result<GraduationAuditDto?>> GetGraduationAuditAsync(long studentId, long semesterId)
    {
        var audit = await _graduationRepo.GetByStudentAndSemesterAsync(studentId, semesterId);
        if (audit == null)
            return Result<GraduationAuditDto?>.Failure("毕业审核记录不存在", 40401);

        return Result<GraduationAuditDto?>.Success(new GraduationAuditDto
        {
            AuditId = audit.AuditId,
            StudentId = audit.StudentId,
            SemesterId = audit.SemesterId,
            TotalCredits = audit.TotalCredits,
            RequiredCredits = audit.RequiredCredits,
            Gpa = audit.Gpa,
            Result = audit.Result.ToString(),
            CertificateNo = audit.CertificateNo,
            DegreeCertNo = audit.DegreeCertNo,
            ReviewedBy = audit.ReviewedBy,
            ReviewedAt = audit.ReviewedAt
        });
    }

    private async Task<StudentProfileDto> MapToDto(StudentProfile profile)
    {
        var college = await _baseDataRepo.GetCollegeByIdAsync(profile.CollegeId);
        var major = await _baseDataRepo.GetMajorByIdAsync(profile.MajorId);
        var @class = profile.ClassId.HasValue ? await _baseDataRepo.GetClassByIdAsync(profile.ClassId.Value) : null;

        return new StudentProfileDto
        {
            StudentId = profile.StudentId,
            StudentNo = profile.StudentNo,
            UserId = profile.UserId,
            CollegeId = profile.CollegeId,
            CollegeName = college?.CollegeName ?? "",
            MajorId = profile.MajorId,
            MajorName = major?.MajorName ?? "",
            ClassId = profile.ClassId,
            ClassName = @class?.ClassName,
            GradeYear = profile.GradeYear,
            EduLevel = profile.EduLevel,
            LengthOfSchool = profile.LengthOfSchool,
            Status = profile.Status.ToString(),
            EnrollmentDate = profile.EnrollmentDate,
            ExpectedGraduation = profile.ExpectedGraduation,
            ActualGraduation = profile.ActualGraduation
        };
    }
}