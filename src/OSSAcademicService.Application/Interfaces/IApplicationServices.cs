using OSSAcademicService.Application.Common;
using OSSAcademicService.Application.DTOs;

namespace OSSAcademicService.Application.Interfaces;

/// <summary>
/// 学生管理服务接口
/// </summary>
public interface IStudentService
{
    Task<Result<StudentProfileDto?>> GetProfileAsync(long studentId);
    Task<Result<StudentProfileDto?>> GetProfileByNoAsync(string studentNo);
    Task<PagedResult<StudentListDto>> GetStudentsPagedAsync(long? collegeId, long? majorId, long? classId, string? status, string? keyword, int page, int pageSize);
    Task<Result<long>> SubmitStatusChangeAsync(long studentId, string changeType, string reason, long appliedBy);
    Task<IReadOnlyList<StatusChangeDto>> GetStatusChangesAsync(long studentId);
    Task<Result<long>> InitiateGraduationAuditAsync(long studentId, long semesterId);
    Task<Result<GraduationAuditDto?>> GetGraduationAuditAsync(long studentId, long semesterId);
}

/// <summary>
/// 课程管理服务接口
/// </summary>
public interface ICourseService
{
    Task<Result<CourseDetailDto?>> GetCourseAsync(long courseId);
    Task<PagedResult<CourseListDto>> GetCoursesPagedAsync(string? keyword, string? courseType, long? collegeId, int page, int pageSize);
    Task<Result<long>> CreateCourseAsync(CreateCourseDto dto);
    Task<Result> UpdateCourseAsync(long courseId, UpdateCourseDto dto);
    Task<Result> DeleteCourseAsync(long courseId);
}

/// <summary>
/// 排课服务接口
/// </summary>
public interface IScheduleService
{
    Task<IReadOnlyList<ScheduleItemDto>> GetScheduleBySemesterAsync(long semesterId);
    Task<IReadOnlyList<ScheduleItemDto>> GetScheduleByTeacherAsync(long teacherId, long semesterId);
    Task<IReadOnlyList<ScheduleItemDto>> GetScheduleByStudentAsync(long studentId, long semesterId);
    Task<Result<long>> AddScheduleItemAsync(AddScheduleItemDto dto);
    Task<Result> RemoveScheduleItemAsync(long itemId);
}

/// <summary>
/// 选课服务接口
/// </summary>
public interface ISelectionService
{
    Task<IReadOnlyList<SelectionRoundDto>> GetRoundsAsync(long semesterId);
    Task<Result<long>> CreateRoundAsync(CreateSelectionRoundDto dto);
    Task<Result> OpenRoundAsync(long roundId);
    Task<Result> CloseRoundAsync(long roundId);
    Task<Result<long>> SelectCourseAsync(long studentId, long taskId, long roundId);
    Task<Result> DropCourseAsync(long studentId, long taskId);
    Task<IReadOnlyList<SelectionRecordDto>> GetMySelectionsAsync(long studentId, long semesterId);
    Task<IReadOnlyList<SelectionRecordDto>> GetTaskSelectionsAsync(long taskId);
}

/// <summary>
/// 成绩服务接口
/// </summary>
public interface IScoreService
{
    Task<PagedResult<ScoreRecordDto>> GetTaskScoresAsync(long taskId, int page, int pageSize);
    Task<Result> EnterScoreAsync(long studentId, long taskId, EnterScoreDto dto, long enteredBy);
    Task<Result> BatchEnterScoresAsync(long taskId, IReadOnlyList<BatchScoreEntryDto> entries, long enteredBy);
    Task<Result> PublishScoresAsync(long taskId, long reviewedBy);
    Task<StudentTranscriptDto> GetTranscriptAsync(long studentId, long? semesterId);
    Task<GpaSummaryDto> GetGpaAsync(long studentId, long? semesterId);
    Task<ScoreAnalysisDto> GetAnalysisAsync(long taskId);
}

/// <summary>
/// 考试服务接口
/// </summary>
public interface IExamService
{
    Task<IReadOnlyList<ExamArrangementDto>> GetExamsAsync(long semesterId, string? examType);
    Task<Result<long>> CreateExamAsync(CreateExamDto dto);
    Task<IReadOnlyList<ExamSeatDto>> GetSeatsAsync(long arrangementId);
    Task<Result> AssignSeatsAsync(long arrangementId);
    Task<Result> AssignInvigilatorsAsync(long arrangementId, IReadOnlyList<long> teacherIds);
    Task<Result<long>> ApplyDeferredExamAsync(long studentId, long arrangementId, string reason, string? proofUrl);
}

/// <summary>
/// 教室服务接口
/// </summary>
public interface IClassroomService
{
    Task<PagedResult<ClassroomDto>> GetClassroomsPagedAsync(long? buildingId, string? roomType, int? minCapacity, int page, int pageSize);
    Task<Result<ClassroomDetailDto?>> GetClassroomAsync(long classroomId);
    Task<IReadOnlyList<ClassroomDto>> GetAvailableClassroomsAsync(long semesterId, int dayOfWeek, int period, int startWeek, int minCapacity);
    Task<Result<long>> BookClassroomAsync(long classroomId, long applicantId, string purpose, DateOnly bookingDate, int startPeriod, int endPeriod);
}

/// <summary>
/// 基础数据服务接口
/// </summary>
public interface IBaseDataService
{
    Task<IReadOnlyList<SemesterDto>> GetSemestersAsync();
    Task<SemesterDto?> GetCurrentSemesterAsync();
    Task<IReadOnlyList<CollegeDto>> GetCollegesAsync();
    Task<IReadOnlyList<MajorDto>> GetMajorsAsync(long collegeId);
    Task<IReadOnlyList<ClassDto>> GetClassesAsync(long majorId);
}