using Microsoft.EntityFrameworkCore;
using OSSAcademicService.Application.Interfaces;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Enums;

namespace OSSAcademicService.Infrastructure.Data.Repositories;

/// <summary>
/// 学生档案仓储实现
/// </summary>
public class StudentProfileRepository : IStudentProfileRepository
{
    private readonly AcademicDbContext _db;

    public StudentProfileRepository(AcademicDbContext db) => _db = db;

    public async Task<StudentProfile?> GetByIdAsync(long studentId, CancellationToken ct = default) =>
        await _db.StudentProfiles.FirstOrDefaultAsync(s => s.StudentId == studentId, ct);

    public async Task<StudentProfile?> GetByStudentNoAsync(string studentNo, CancellationToken ct = default) =>
        await _db.StudentProfiles.FirstOrDefaultAsync(s => s.StudentNo == studentNo, ct);

    public async Task<StudentProfile?> GetByUserIdAsync(long userId, CancellationToken ct = default) =>
        await _db.StudentProfiles.FirstOrDefaultAsync(s => s.UserId == userId, ct);

    public async Task<(IReadOnlyList<StudentProfile> Items, long TotalCount)> GetPagedAsync(
        long? collegeId, long? majorId, long? classId,
        StudentStatus? status, string? keyword,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.StudentProfiles.AsQueryable();

        if (collegeId.HasValue) query = query.Where(s => s.CollegeId == collegeId.Value);
        if (majorId.HasValue) query = query.Where(s => s.MajorId == majorId.Value);
        if (classId.HasValue) query = query.Where(s => s.ClassId == classId.Value);
        if (status.HasValue) query = query.Where(s => s.Status == status.Value);
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(s => s.StudentNo.Contains(keyword));

        var totalCount = await query.LongCountAsync(ct);
        var items = await query.OrderBy(s => s.StudentNo)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, totalCount);
    }

    public void Add(StudentProfile profile) => _db.StudentProfiles.Add(profile);
    public void Update(StudentProfile profile) => _db.StudentProfiles.Update(profile);
}

/// <summary>
/// 学籍异动仓储实现
/// </summary>
public class StatusChangeRepository : IStatusChangeRepository
{
    private readonly AcademicDbContext _db;

    public StatusChangeRepository(AcademicDbContext db) => _db = db;

    public async Task<StatusChange?> GetByIdAsync(long changeId, CancellationToken ct = default) =>
        await _db.StatusChanges.FirstOrDefaultAsync(s => s.ChangeId == changeId, ct);

    public async Task<IReadOnlyList<StatusChange>> GetByStudentAsync(long studentId, CancellationToken ct = default) =>
        await _db.StatusChanges.Where(s => s.StudentId == studentId).OrderByDescending(s => s.CreatedAt).ToListAsync(ct);

    public void Add(StatusChange change) => _db.StatusChanges.Add(change);
    public void Update(StatusChange change) => _db.StatusChanges.Update(change);
}

/// <summary>
/// 毕业审核仓储实现
/// </summary>
public class GraduationAuditRepository : IGraduationAuditRepository
{
    private readonly AcademicDbContext _db;

    public GraduationAuditRepository(AcademicDbContext db) => _db = db;

    public async Task<GraduationAudit?> GetByIdAsync(long auditId, CancellationToken ct = default) =>
        await _db.GraduationAudits.FirstOrDefaultAsync(g => g.AuditId == auditId, ct);

    public async Task<GraduationAudit?> GetByStudentAndSemesterAsync(long studentId, long semesterId, CancellationToken ct = default) =>
        await _db.GraduationAudits.FirstOrDefaultAsync(g => g.StudentId == studentId && g.SemesterId == semesterId, ct);

    public void Add(GraduationAudit audit) => _db.GraduationAudits.Add(audit);
    public void Update(GraduationAudit audit) => _db.GraduationAudits.Update(audit);
}

/// <summary>
/// 课程仓储实现
/// </summary>
public class CourseRepository : ICourseRepository
{
    private readonly AcademicDbContext _db;

    public CourseRepository(AcademicDbContext db) => _db = db;

    public async Task<Course?> GetByIdAsync(long courseId, CancellationToken ct = default) =>
        await _db.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId, ct);

    public async Task<Course?> GetByCodeAsync(string courseCode, CancellationToken ct = default) =>
        await _db.Courses.FirstOrDefaultAsync(c => c.CourseCode == courseCode, ct);

    public async Task<(IReadOnlyList<Course> Items, long TotalCount)> GetPagedAsync(
        string? keyword, string? courseType, long? collegeId,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Courses.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(c => c.CourseName.Contains(keyword) || c.CourseCode.Contains(keyword));
        if (!string.IsNullOrEmpty(courseType))
            query = query.Where(c => c.CourseType == courseType);
        if (collegeId.HasValue)
            query = query.Where(c => c.CollegeId == collegeId.Value);

        var totalCount = await query.LongCountAsync(ct);
        var items = await query.OrderBy(c => c.CourseCode)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<Course>> GetByIdsAsync(IEnumerable<long> courseIds, CancellationToken ct = default) =>
        await _db.Courses.Where(c => courseIds.Contains(c.CourseId)).ToListAsync(ct);

    public void Add(Course course) => _db.Courses.Add(course);
    public void Update(Course course) => _db.Courses.Update(course);
}

/// <summary>
/// 学期仓储实现
/// </summary>
public class SemesterRepository : ISemesterRepository
{
    private readonly AcademicDbContext _db;

    public SemesterRepository(AcademicDbContext db) => _db = db;

    public async Task<Semester?> GetCurrentAsync(CancellationToken ct = default) =>
        await _db.Semesters.FirstOrDefaultAsync(s => s.IsCurrent, ct);

    public async Task<Semester?> GetByIdAsync(long semesterId, CancellationToken ct = default) =>
        await _db.Semesters.FirstOrDefaultAsync(s => s.SemesterId == semesterId, ct);

    public async Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Semesters.OrderByDescending(s => s.StartDate).ToListAsync(ct);

    public void Add(Semester semester) => _db.Semesters.Add(semester);
    public void Update(Semester semester) => _db.Semesters.Update(semester);
}

/// <summary>
/// 教学任务仓储实现
/// </summary>
public class TeachingTaskRepository : ITeachingTaskRepository
{
    private readonly AcademicDbContext _db;

    public TeachingTaskRepository(AcademicDbContext db) => _db = db;

    public async Task<TeachingTask?> GetByIdAsync(long taskId, CancellationToken ct = default) =>
        await _db.TeachingTasks.FirstOrDefaultAsync(t => t.TaskId == taskId, ct);

    public async Task<IReadOnlyList<TeachingTask>> GetBySemesterAsync(long semesterId, CancellationToken ct = default) =>
        await _db.TeachingTasks.Where(t => t.SemesterId == semesterId).ToListAsync(ct);

    public async Task<IReadOnlyList<TeachingTask>> GetByTeacherAsync(long teacherId, long semesterId, CancellationToken ct = default) =>
        await _db.TeachingTasks.Where(t => t.TeacherId == teacherId && t.SemesterId == semesterId).ToListAsync(ct);

    public void Add(TeachingTask task) => _db.TeachingTasks.Add(task);
    public void Update(TeachingTask task) => _db.TeachingTasks.Update(task);
}

/// <summary>
/// 排课仓储实现
/// </summary>
public class ScheduleRepository : IScheduleRepository
{
    private readonly AcademicDbContext _db;

    public ScheduleRepository(AcademicDbContext db) => _db = db;

    public async Task<IReadOnlyList<ScheduleItem>> GetBySemesterAsync(long semesterId, CancellationToken ct = default) =>
        await _db.ScheduleItems.Where(s => s.SemesterId == semesterId).ToListAsync(ct);

    public async Task<IReadOnlyList<ScheduleItem>> GetByTeacherAsync(long teacherId, long semesterId, CancellationToken ct = default) =>
        await _db.ScheduleItems.Where(s => s.TeacherId == teacherId && s.SemesterId == semesterId).ToListAsync(ct);

    public async Task<IReadOnlyList<ScheduleItem>> GetByClassroomAsync(long classroomId, long semesterId, CancellationToken ct = default) =>
        await _db.ScheduleItems.Where(s => s.ClassroomId == classroomId && s.SemesterId == semesterId).ToListAsync(ct);

    public async Task<IReadOnlyList<ScheduleItem>> GetByTaskAsync(long taskId, CancellationToken ct = default) =>
        await _db.ScheduleItems.Where(s => s.TaskId == taskId).ToListAsync(ct);

    public async Task<ScheduleItem?> GetByIdAsync(long itemId, CancellationToken ct = default) =>
        await _db.ScheduleItems.FirstOrDefaultAsync(s => s.ItemId == itemId, ct);

    public void Add(ScheduleItem item) => _db.ScheduleItems.Add(item);
    public void Update(ScheduleItem item) => _db.ScheduleItems.Update(item);
    public void Remove(ScheduleItem item) => _db.ScheduleItems.Remove(item);
}

/// <summary>
/// 选课仓储实现
/// </summary>
public class SelectionRepository : ISelectionRepository
{
    private readonly AcademicDbContext _db;

    public SelectionRepository(AcademicDbContext db) => _db = db;

    public async Task<IReadOnlyList<SelectionRecord>> GetByStudentAsync(long studentId, long semesterId, CancellationToken ct = default) =>
        await _db.SelectionRecords.Where(s => s.StudentId == studentId && s.SemesterId == semesterId).ToListAsync(ct);

    public async Task<IReadOnlyList<SelectionRecord>> GetByTaskAsync(long taskId, CancellationToken ct = default) =>
        await _db.SelectionRecords.Where(s => s.TaskId == taskId).ToListAsync(ct);

    public async Task<SelectionRecord?> GetByStudentAndTaskAsync(long studentId, long taskId, CancellationToken ct = default) =>
        await _db.SelectionRecords.FirstOrDefaultAsync(s => s.StudentId == studentId && s.TaskId == taskId, ct);

    public async Task<bool> ExistsAsync(long studentId, long taskId, CancellationToken ct = default) =>
        await _db.SelectionRecords.AnyAsync(s => s.StudentId == studentId && s.TaskId == taskId, ct);

    public async Task<int> GetCurrentCountAsync(long taskId, CancellationToken ct = default) =>
        await _db.SelectionRecords.CountAsync(s => s.TaskId == taskId && s.Status == "已选", ct);

    public async Task<SelectionRound?> GetRoundByIdAsync(long roundId, CancellationToken ct = default) =>
        await _db.SelectionRounds.FirstOrDefaultAsync(r => r.RoundId == roundId, ct);

    public async Task<IReadOnlyList<SelectionRound>> GetRoundsBySemesterAsync(long semesterId, CancellationToken ct = default) =>
        await _db.SelectionRounds.Where(r => r.SemesterId == semesterId).OrderBy(r => r.StartTime).ToListAsync(ct);

    public void Add(SelectionRecord record) => _db.SelectionRecords.Add(record);
    public void Update(SelectionRecord record) => _db.SelectionRecords.Update(record);
    public void AddRound(SelectionRound round) => _db.SelectionRounds.Add(round);
    public void UpdateRound(SelectionRound round) => _db.SelectionRounds.Update(round);
}

/// <summary>
/// 成绩仓储实现
/// </summary>
public class ScoreRepository : IScoreRepository
{
    private readonly AcademicDbContext _db;

    public ScoreRepository(AcademicDbContext db) => _db = db;

    public async Task<ScoreRecord?> GetByIdAsync(long recordId, CancellationToken ct = default) =>
        await _db.ScoreRecords.FirstOrDefaultAsync(s => s.RecordId == recordId, ct);

    public async Task<ScoreRecord?> GetByStudentAndTaskAsync(long studentId, long taskId, CancellationToken ct = default) =>
        await _db.ScoreRecords.FirstOrDefaultAsync(s => s.StudentId == studentId && s.TaskId == taskId, ct);

    public async Task<IReadOnlyList<ScoreRecord>> GetByTaskAsync(long taskId, CancellationToken ct = default) =>
        await _db.ScoreRecords.Where(s => s.TaskId == taskId).ToListAsync(ct);

    public async Task<IReadOnlyList<ScoreRecord>> GetByStudentAsync(long studentId, long? semesterId, CancellationToken ct = default)
    {
        var query = _db.ScoreRecords.Where(s => s.StudentId == studentId);
        if (semesterId.HasValue) query = query.Where(s => s.SemesterId == semesterId.Value);
        return await query.OrderByDescending(s => s.SemesterId).ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<ScoreRecord> Items, long TotalCount)> GetPagedAsync(
        long? taskId, long? studentId, bool? isPublished,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.ScoreRecords.AsQueryable();

        if (taskId.HasValue) query = query.Where(s => s.TaskId == taskId.Value);
        if (studentId.HasValue) query = query.Where(s => s.StudentId == studentId.Value);
        if (isPublished.HasValue) query = query.Where(s => s.IsPublished == isPublished.Value);

        var totalCount = await query.LongCountAsync(ct);
        var items = await query.OrderBy(s => s.StudentId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<ScoreRule?> GetRuleByCourseAsync(long courseId, CancellationToken ct = default) =>
        await _db.ScoreRules.FirstOrDefaultAsync(r => r.CourseId == courseId, ct);

    public async Task<GpaSummary?> GetGpaSummaryAsync(long studentId, long? semesterId, CancellationToken ct = default) =>
        await _db.GpaSummaries.FirstOrDefaultAsync(g => g.StudentId == studentId && g.SemesterId == semesterId, ct);

    public void Add(ScoreRecord record) => _db.ScoreRecords.Add(record);
    public void Update(ScoreRecord record) => _db.ScoreRecords.Update(record);

    public void AddOrUpdateGpaSummary(GpaSummary summary)
    {
        var existing = _db.GpaSummaries.FirstOrDefault(g => g.StudentId == summary.StudentId && g.SemesterId == summary.SemesterId);
        if (existing == null) _db.GpaSummaries.Add(summary);
        else existing.Update(summary.TotalCredits, summary.TotalPoints, summary.Gpa);
    }

    public void AddOrUpdateScoreRule(ScoreRule rule)
    {
        var existing = _db.ScoreRules.FirstOrDefault(r => r.CourseId == rule.CourseId);
        if (existing == null) _db.ScoreRules.Add(rule);
        else existing.Update(rule.RegularWeight, rule.ExperimentWeight, rule.MidWeight, rule.FinalWeight, rule.ScoreType);
    }
}

/// <summary>
/// 考试仓储实现
/// </summary>
public class ExamRepository : IExamRepository
{
    private readonly AcademicDbContext _db;

    public ExamRepository(AcademicDbContext db) => _db = db;

    public async Task<ExamArrangement?> GetByIdAsync(long arrangementId, CancellationToken ct = default) =>
        await _db.ExamArrangements.FirstOrDefaultAsync(e => e.ArrangementId == arrangementId, ct);

    public async Task<IReadOnlyList<ExamArrangement>> GetBySemesterAsync(long semesterId, string? examType, CancellationToken ct = default)
    {
        var query = _db.ExamArrangements.Where(e => e.SemesterId == semesterId);
        if (!string.IsNullOrEmpty(examType)) query = query.Where(e => e.ExamType == examType);
        return await query.OrderBy(e => e.ExamDate).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ExamSeat>> GetSeatsByArrangementAsync(long arrangementId, CancellationToken ct = default) =>
        await _db.ExamSeats.Where(e => e.ArrangementId == arrangementId).OrderBy(e => e.SeatNo).ToListAsync(ct);

    public async Task<IReadOnlyList<Invigilation>> GetInvigilationsByArrangementAsync(long arrangementId, CancellationToken ct = default) =>
        await _db.Invigilations.Where(e => e.ArrangementId == arrangementId).ToListAsync(ct);

    public async Task<DeferredExam?> GetDeferredByIdAsync(long deferredId, CancellationToken ct = default) =>
        await _db.DeferredExams.FirstOrDefaultAsync(d => d.DeferredId == deferredId, ct);

    public async Task<IReadOnlyList<DeferredExam>> GetDeferredByStudentAsync(long studentId, CancellationToken ct = default) =>
        await _db.DeferredExams.Where(d => d.StudentId == studentId).ToListAsync(ct);

    public void AddArrangement(ExamArrangement arrangement) => _db.ExamArrangements.Add(arrangement);
    public void UpdateArrangement(ExamArrangement arrangement) => _db.ExamArrangements.Update(arrangement);
    public void AddSeat(ExamSeat seat) => _db.ExamSeats.Add(seat);
    public void AddInvigilation(Invigilation invigilation) => _db.Invigilations.Add(invigilation);
    public void AddDeferred(DeferredExam deferred) => _db.DeferredExams.Add(deferred);
    public void UpdateDeferred(DeferredExam deferred) => _db.DeferredExams.Update(deferred);
}

/// <summary>
/// 教室仓储实现
/// </summary>
public class ClassroomRepository : IClassroomRepository
{
    private readonly AcademicDbContext _db;

    public ClassroomRepository(AcademicDbContext db) => _db = db;

    public async Task<Classroom?> GetByIdAsync(long classroomId, CancellationToken ct = default) =>
        await _db.Classrooms.FirstOrDefaultAsync(c => c.ClassroomId == classroomId, ct);

    public async Task<IReadOnlyList<Classroom>> GetAvailableAsync(long semesterId, int dayOfWeek, int period, int startWeek, int minCapacity, CancellationToken ct = default) =>
        await _db.Classrooms.Where(c => c.Capacity >= minCapacity && c.Status == "正常").ToListAsync(ct);

    public async Task<(IReadOnlyList<Classroom> Items, long TotalCount)> GetPagedAsync(
        long? buildingId, string? roomType, int? minCapacity,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Classrooms.AsQueryable();

        if (buildingId.HasValue) query = query.Where(c => c.BuildingId == buildingId.Value);
        if (!string.IsNullOrEmpty(roomType)) query = query.Where(c => c.RoomType == roomType);
        if (minCapacity.HasValue) query = query.Where(c => c.Capacity >= minCapacity.Value);

        var totalCount = await query.LongCountAsync(ct);
        var items = await query.OrderBy(c => c.RoomNo)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<Building?> GetBuildingByIdAsync(long buildingId, CancellationToken ct = default) =>
        await _db.Buildings.FirstOrDefaultAsync(b => b.BuildingId == buildingId, ct);

    public async Task<IReadOnlyList<Building>> GetAllBuildingsAsync(CancellationToken ct = default) =>
        await _db.Buildings.OrderBy(b => b.BuildingCode).ToListAsync(ct);

    public async Task<ClassroomBooking?> GetBookingByIdAsync(long bookingId, CancellationToken ct = default) =>
        await _db.ClassroomBookings.FirstOrDefaultAsync(b => b.BookingId == bookingId, ct);

    public async Task<IReadOnlyList<ClassroomBooking>> GetBookingsByClassroomAsync(long classroomId, DateOnly date, CancellationToken ct = default) =>
        await _db.ClassroomBookings.Where(b => b.ClassroomId == classroomId && b.BookingDate == date).ToListAsync(ct);

    public void Add(Classroom classroom) => _db.Classrooms.Add(classroom);
    public void Update(Classroom classroom) => _db.Classrooms.Update(classroom);
    public void AddBuilding(Building building) => _db.Buildings.Add(building);
    public void AddBooking(ClassroomBooking booking) => _db.ClassroomBookings.Add(booking);
    public void UpdateBooking(ClassroomBooking booking) => _db.ClassroomBookings.Update(booking);
}

/// <summary>
/// 基础数据仓储实现
/// </summary>
public class BaseDataRepository : IBaseDataRepository
{
    private readonly AcademicDbContext _db;

    public BaseDataRepository(AcademicDbContext db) => _db = db;

    public async Task<IReadOnlyList<College>> GetAllCollegesAsync(CancellationToken ct = default) =>
        await _db.Colleges.OrderBy(c => c.SortOrder).ToListAsync(ct);

    public async Task<College?> GetCollegeByIdAsync(long collegeId, CancellationToken ct = default) =>
        await _db.Colleges.FirstOrDefaultAsync(c => c.CollegeId == collegeId, ct);

    public async Task<IReadOnlyList<Major>> GetMajorsByCollegeAsync(long collegeId, CancellationToken ct = default) =>
        await _db.Majors.Where(m => m.CollegeId == collegeId).ToListAsync(ct);

    public async Task<Major?> GetMajorByIdAsync(long majorId, CancellationToken ct = default) =>
        await _db.Majors.FirstOrDefaultAsync(m => m.MajorId == majorId, ct);

    public async Task<IReadOnlyList<Class>> GetClassesByMajorAsync(long majorId, CancellationToken ct = default) =>
        await _db.Classes.Where(c => c.MajorId == majorId).ToListAsync(ct);

    public async Task<Class?> GetClassByIdAsync(long classId, CancellationToken ct = default) =>
        await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == classId, ct);

    public void AddCollege(College college) => _db.Colleges.Add(college);
    public void AddMajor(Major major) => _db.Majors.Add(major);
    public void AddClass(Class @class) => _db.Classes.Add(@class);
}

/// <summary>
/// 工作单元实现
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AcademicDbContext _db;

    public UnitOfWork(AcademicDbContext db) => _db = db;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}