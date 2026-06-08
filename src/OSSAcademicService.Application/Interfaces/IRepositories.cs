using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Enums;

namespace OSSAcademicService.Application.Interfaces;

/// <summary>
/// 学生档案仓储接口
/// </summary>
public interface IStudentProfileRepository
{
    Task<StudentProfile?> GetByIdAsync(long studentId, CancellationToken ct = default);
    Task<StudentProfile?> GetByStudentNoAsync(string studentNo, CancellationToken ct = default);
    Task<StudentProfile?> GetByUserIdAsync(long userId, CancellationToken ct = default);
    Task<(IReadOnlyList<StudentProfile> Items, long TotalCount)> GetPagedAsync(
        long? collegeId, long? majorId, long? classId,
        StudentStatus? status, string? keyword,
        int page, int pageSize, CancellationToken ct = default);
    void Add(StudentProfile profile);
    void Update(StudentProfile profile);
}

/// <summary>
/// 学籍异动仓储接口
/// </summary>
public interface IStatusChangeRepository
{
    Task<StatusChange?> GetByIdAsync(long changeId, CancellationToken ct = default);
    Task<IReadOnlyList<StatusChange>> GetByStudentAsync(long studentId, CancellationToken ct = default);
    void Add(StatusChange change);
    void Update(StatusChange change);
}

/// <summary>
/// 毕业审核仓储接口
/// </summary>
public interface IGraduationAuditRepository
{
    Task<GraduationAudit?> GetByIdAsync(long auditId, CancellationToken ct = default);
    Task<GraduationAudit?> GetByStudentAndSemesterAsync(long studentId, long semesterId, CancellationToken ct = default);
    void Add(GraduationAudit audit);
    void Update(GraduationAudit audit);
}

/// <summary>
/// 课程仓储接口
/// </summary>
public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(long courseId, CancellationToken ct = default);
    Task<Course?> GetByCodeAsync(string courseCode, CancellationToken ct = default);
    Task<(IReadOnlyList<Course> Items, long TotalCount)> GetPagedAsync(
        string? keyword, string? courseType, long? collegeId,
        int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetByIdsAsync(IEnumerable<long> courseIds, CancellationToken ct = default);
    void Add(Course course);
    void Update(Course course);
}

/// <summary>
/// 学期仓储接口
/// </summary>
public interface ISemesterRepository
{
    Task<Semester?> GetCurrentAsync(CancellationToken ct = default);
    Task<Semester?> GetByIdAsync(long semesterId, CancellationToken ct = default);
    Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken ct = default);
    void Add(Semester semester);
    void Update(Semester semester);
}

/// <summary>
/// 教学任务仓储接口
/// </summary>
public interface ITeachingTaskRepository
{
    Task<TeachingTask?> GetByIdAsync(long taskId, CancellationToken ct = default);
    Task<IReadOnlyList<TeachingTask>> GetBySemesterAsync(long semesterId, CancellationToken ct = default);
    Task<IReadOnlyList<TeachingTask>> GetByTeacherAsync(long teacherId, long semesterId, CancellationToken ct = default);
    void Add(TeachingTask task);
    void Update(TeachingTask task);
}

/// <summary>
/// 排课仓储接口
/// </summary>
public interface IScheduleRepository
{
    Task<IReadOnlyList<ScheduleItem>> GetBySemesterAsync(long semesterId, CancellationToken ct = default);
    Task<IReadOnlyList<ScheduleItem>> GetByTeacherAsync(long teacherId, long semesterId, CancellationToken ct = default);
    Task<IReadOnlyList<ScheduleItem>> GetByClassroomAsync(long classroomId, long semesterId, CancellationToken ct = default);
    Task<IReadOnlyList<ScheduleItem>> GetByTaskAsync(long taskId, CancellationToken ct = default);
    Task<ScheduleItem?> GetByIdAsync(long itemId, CancellationToken ct = default);
    void Add(ScheduleItem item);
    void Update(ScheduleItem item);
    void Remove(ScheduleItem item);
}

/// <summary>
/// 选课仓储接口
/// </summary>
public interface ISelectionRepository
{
    Task<IReadOnlyList<SelectionRecord>> GetByStudentAsync(long studentId, long semesterId, CancellationToken ct = default);
    Task<IReadOnlyList<SelectionRecord>> GetByTaskAsync(long taskId, CancellationToken ct = default);
    Task<SelectionRecord?> GetByStudentAndTaskAsync(long studentId, long taskId, CancellationToken ct = default);
    Task<bool> ExistsAsync(long studentId, long taskId, CancellationToken ct = default);
    Task<int> GetCurrentCountAsync(long taskId, CancellationToken ct = default);
    Task<SelectionRound?> GetRoundByIdAsync(long roundId, CancellationToken ct = default);
    Task<IReadOnlyList<SelectionRound>> GetRoundsBySemesterAsync(long semesterId, CancellationToken ct = default);
    void Add(SelectionRecord record);
    void Update(SelectionRecord record);
    void AddRound(SelectionRound round);
    void UpdateRound(SelectionRound round);
}

/// <summary>
/// 成绩仓储接口
/// </summary>
public interface IScoreRepository
{
    Task<ScoreRecord?> GetByIdAsync(long recordId, CancellationToken ct = default);
    Task<ScoreRecord?> GetByStudentAndTaskAsync(long studentId, long taskId, CancellationToken ct = default);
    Task<IReadOnlyList<ScoreRecord>> GetByTaskAsync(long taskId, CancellationToken ct = default);
    Task<IReadOnlyList<ScoreRecord>> GetByStudentAsync(long studentId, long? semesterId, CancellationToken ct = default);
    Task<(IReadOnlyList<ScoreRecord> Items, long TotalCount)> GetPagedAsync(
        long? taskId, long? studentId, bool? isPublished,
        int page, int pageSize, CancellationToken ct = default);
    Task<ScoreRule?> GetRuleByCourseAsync(long courseId, CancellationToken ct = default);
    Task<GpaSummary?> GetGpaSummaryAsync(long studentId, long? semesterId, CancellationToken ct = default);
    void Add(ScoreRecord record);
    void Update(ScoreRecord record);
    void AddOrUpdateGpaSummary(GpaSummary summary);
    void AddOrUpdateScoreRule(ScoreRule rule);
}

/// <summary>
/// 考试仓储接口
/// </summary>
public interface IExamRepository
{
    Task<ExamArrangement?> GetByIdAsync(long arrangementId, CancellationToken ct = default);
    Task<IReadOnlyList<ExamArrangement>> GetBySemesterAsync(long semesterId, string? examType, CancellationToken ct = default);
    Task<IReadOnlyList<ExamSeat>> GetSeatsByArrangementAsync(long arrangementId, CancellationToken ct = default);
    Task<IReadOnlyList<Invigilation>> GetInvigilationsByArrangementAsync(long arrangementId, CancellationToken ct = default);
    Task<DeferredExam?> GetDeferredByIdAsync(long deferredId, CancellationToken ct = default);
    Task<IReadOnlyList<DeferredExam>> GetDeferredByStudentAsync(long studentId, CancellationToken ct = default);
    void AddArrangement(ExamArrangement arrangement);
    void UpdateArrangement(ExamArrangement arrangement);
    void AddSeat(ExamSeat seat);
    void AddInvigilation(Invigilation invigilation);
    void AddDeferred(DeferredExam deferred);
    void UpdateDeferred(DeferredExam deferred);
}

/// <summary>
/// 教室仓储接口
/// </summary>
public interface IClassroomRepository
{
    Task<Classroom?> GetByIdAsync(long classroomId, CancellationToken ct = default);
    Task<IReadOnlyList<Classroom>> GetAvailableAsync(long semesterId, int dayOfWeek, int period, int startWeek, int minCapacity, CancellationToken ct = default);
    Task<(IReadOnlyList<Classroom> Items, long TotalCount)> GetPagedAsync(
        long? buildingId, string? roomType, int? minCapacity,
        int page, int pageSize, CancellationToken ct = default);
    Task<Building?> GetBuildingByIdAsync(long buildingId, CancellationToken ct = default);
    Task<IReadOnlyList<Building>> GetAllBuildingsAsync(CancellationToken ct = default);
    Task<ClassroomBooking?> GetBookingByIdAsync(long bookingId, CancellationToken ct = default);
    Task<IReadOnlyList<ClassroomBooking>> GetBookingsByClassroomAsync(long classroomId, DateOnly date, CancellationToken ct = default);
    void Add(Classroom classroom);
    void Update(Classroom classroom);
    void AddBuilding(Building building);
    void AddBooking(ClassroomBooking booking);
    void UpdateBooking(ClassroomBooking booking);
}

/// <summary>
/// 基础数据仓储接口
/// </summary>
public interface IBaseDataRepository
{
    Task<IReadOnlyList<College>> GetAllCollegesAsync(CancellationToken ct = default);
    Task<College?> GetCollegeByIdAsync(long collegeId, CancellationToken ct = default);
    Task<IReadOnlyList<Major>> GetMajorsByCollegeAsync(long collegeId, CancellationToken ct = default);
    Task<Major?> GetMajorByIdAsync(long majorId, CancellationToken ct = default);
    Task<IReadOnlyList<Class>> GetClassesByMajorAsync(long majorId, CancellationToken ct = default);
    Task<Class?> GetClassByIdAsync(long classId, CancellationToken ct = default);
    void AddCollege(College college);
    void AddMajor(Major major);
    void AddClass(Class @class);
}

/// <summary>
/// 工作单元接口
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}