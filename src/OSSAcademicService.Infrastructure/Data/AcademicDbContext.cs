using Microsoft.EntityFrameworkCore;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Infrastructure.Data;

/// <summary>
/// 教务管理数据库上下文
/// </summary>
public class AcademicDbContext : DbContext
{
    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();
    public DbSet<StatusChange> StatusChanges => Set<StatusChange>();
    public DbSet<GraduationAudit> GraduationAudits => Set<GraduationAudit>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CoursePrerequisite> CoursePrerequisites => Set<CoursePrerequisite>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();
    public DbSet<PlanCourse> PlanCourses => Set<PlanCourse>();
    public DbSet<TeachingTask> TeachingTasks => Set<TeachingTask>();
    public DbSet<ScheduleItem> ScheduleItems => Set<ScheduleItem>();
    public DbSet<ScheduleAdjustment> ScheduleAdjustments => Set<ScheduleAdjustment>();
    public DbSet<SelectionRound> SelectionRounds => Set<SelectionRound>();
    public DbSet<SelectionRecord> SelectionRecords => Set<SelectionRecord>();
    public DbSet<ScoreRecord> ScoreRecords => Set<ScoreRecord>();
    public DbSet<ScoreRule> ScoreRules => Set<ScoreRule>();
    public DbSet<GpaSummary> GpaSummaries => Set<GpaSummary>();
    public DbSet<ExamArrangement> ExamArrangements => Set<ExamArrangement>();
    public DbSet<ExamSeat> ExamSeats => Set<ExamSeat>();
    public DbSet<Invigilation> Invigilations => Set<Invigilation>();
    public DbSet<DeferredExam> DeferredExams => Set<DeferredExam>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<ClassroomBooking> ClassroomBookings => Set<ClassroomBooking>();
    public DbSet<College> Colleges => Set<College>();
    public DbSet<Major> Majors => Set<Major>();
    public DbSet<Class> Classes => Set<Class>();

    public AcademicDbContext(DbContextOptions<AcademicDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicDbContext).Assembly);

        modelBuilder.Entity<StudentProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Course>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TeachingTask>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ScoreRecord>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Classroom>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Building>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<College>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Major>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Class>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<TrainingPlan>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<StatusChange>().HasQueryFilter(e => !e.IsDeleted);
    }
}