using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OSSAcademicService.Domain.Entities;
using OSSAcademicService.Domain.Enums;

namespace OSSAcademicService.Infrastructure.Data.Configurations;

/// <summary>
/// 实体配置
/// </summary>
public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.ToTable("t_student_profile");
        builder.HasKey(e => e.StudentId);
        builder.Property(e => e.StudentId).HasColumnName("student_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentNo).HasColumnName("student_no").HasMaxLength(20).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.CollegeId).HasColumnName("college_id").IsRequired();
        builder.Property(e => e.MajorId).HasColumnName("major_id").IsRequired();
        builder.Property(e => e.ClassId).HasColumnName("class_id");
        builder.Property(e => e.GradeYear).HasColumnName("grade_year").IsRequired();
        builder.Property(e => e.EduLevel).HasColumnName("edu_level").HasMaxLength(20).IsRequired();
        builder.Property(e => e.LengthOfSchool).HasColumnName("length_of_school").IsRequired();
        builder.Property(e => e.TrainingPlanId).HasColumnName("training_plan_id");
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        builder.Property(e => e.EnrollmentDate).HasColumnName("enrollment_date");
        builder.Property(e => e.ExpectedGraduation).HasColumnName("expected_graduation");
        builder.Property(e => e.ActualGraduation).HasColumnName("actual_graduation");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.StudentNo).IsUnique().HasDatabaseName("uk_student_no");
        builder.HasIndex(e => e.UserId).HasDatabaseName("idx_user_id");
        builder.HasIndex(e => new { e.CollegeId, e.MajorId }).HasDatabaseName("idx_college_major");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_status");
        builder.Ignore(e => e.DomainEvents);
        builder.OwnsMany(e => e.StatusChanges, sc => sc.ToTable("t_status_change"));
    }
}

public class StatusChangeConfiguration : IEntityTypeConfiguration<StatusChange>
{
    public void Configure(EntityTypeBuilder<StatusChange> builder)
    {
        builder.ToTable("t_status_change");
        builder.HasKey(e => e.ChangeId);
        builder.Property(e => e.ChangeId).HasColumnName("change_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.ChangeType).HasColumnName("change_type").HasConversion<int>().IsRequired();
        builder.Property(e => e.ChangeReason).HasColumnName("change_reason").HasColumnType("TEXT").IsRequired();
        builder.Property(e => e.OldValue).HasColumnName("old_value").HasColumnType("JSON");
        builder.Property(e => e.NewValue).HasColumnName("new_value").HasColumnType("JSON");
        builder.Property(e => e.ApplyStatus).HasColumnName("apply_status").HasConversion<int>().IsRequired();
        builder.Property(e => e.AppliedBy).HasColumnName("applied_by");
        builder.Property(e => e.AppliedAt).HasColumnName("applied_at");
        builder.Property(e => e.EffectiveDate).HasColumnName("effective_date");
        builder.Property(e => e.Attachments).HasColumnName("attachments").HasColumnType("JSON");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.StudentId).HasDatabaseName("idx_student_id");
        builder.HasIndex(e => e.ChangeType).HasDatabaseName("idx_change_type");
        builder.HasIndex(e => e.ApplyStatus).HasDatabaseName("idx_apply_status");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class GraduationAuditConfiguration : IEntityTypeConfiguration<GraduationAudit>
{
    public void Configure(EntityTypeBuilder<GraduationAudit> builder)
    {
        builder.ToTable("t_graduation_audit");
        builder.HasKey(e => e.AuditId);
        builder.Property(e => e.AuditId).HasColumnName("audit_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.TotalCredits).HasColumnName("total_credits").HasPrecision(5, 1).HasDefaultValue(0);
        builder.Property(e => e.RequiredCredits).HasColumnName("required_credits").HasPrecision(5, 1).IsRequired();
        builder.Property(e => e.RequiredCourses).HasColumnName("required_courses").HasColumnType("JSON");
        builder.Property(e => e.ElectiveCredits).HasColumnName("elective_credits").HasPrecision(5, 1).HasDefaultValue(0);
        builder.Property(e => e.Gpa).HasColumnName("gpa").HasPrecision(3, 2);
        builder.Property(e => e.DisciplinaryCheck).HasColumnName("disciplinary_check").HasDefaultValue(true);
        builder.Property(e => e.Result).HasColumnName("result").HasConversion<int>().IsRequired();
        builder.Property(e => e.CertificateNo).HasColumnName("certificate_no").HasMaxLength(50);
        builder.Property(e => e.DegreeCertNo).HasColumnName("degree_cert_no").HasMaxLength(50);
        builder.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
        builder.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.StudentId).HasDatabaseName("idx_student_id");
        builder.HasIndex(e => new { e.StudentId, e.SemesterId }).IsUnique().HasDatabaseName("uk_student_semester");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("t_course");
        builder.HasKey(e => e.CourseId);
        builder.Property(e => e.CourseId).HasColumnName("course_id").ValueGeneratedOnAdd();
        builder.Property(e => e.CourseCode).HasColumnName("course_code").HasMaxLength(30).IsRequired();
        builder.Property(e => e.CourseName).HasColumnName("course_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.CourseNameEn).HasColumnName("course_name_en").HasMaxLength(200);
        builder.Property(e => e.Credit).HasColumnName("credit").HasPrecision(3, 1).IsRequired();
        builder.Property(e => e.TotalHours).HasColumnName("total_hours").IsRequired();
        builder.Property(e => e.TeachingHours).HasColumnName("teaching_hours").HasDefaultValue(0);
        builder.Property(e => e.LabHours).HasColumnName("lab_hours").HasDefaultValue(0);
        builder.Property(e => e.PracticeHours).HasColumnName("practice_hours").HasDefaultValue(0);
        builder.Property(e => e.TeachingMode).HasColumnName("teaching_mode").HasMaxLength(20).IsRequired();
        builder.Property(e => e.CourseType).HasColumnName("course_type").HasMaxLength(30).IsRequired();
        builder.Property(e => e.CollegeId).HasColumnName("college_id").IsRequired();
        builder.Property(e => e.OutlineUrl).HasColumnName("outline_url").HasMaxLength(500);
        builder.Property(e => e.MaxCapacity).HasColumnName("max_capacity");
        builder.Property(e => e.AssessMethod).HasColumnName("assess_method").HasMaxLength(20).HasDefaultValue("考试");
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.CourseCode).IsUnique().HasDatabaseName("uk_course_code");
        builder.HasIndex(e => e.CollegeId).HasDatabaseName("idx_college_id");
        builder.HasIndex(e => e.CourseType).HasDatabaseName("idx_course_type");
        builder.Ignore(e => e.DomainEvents);
        builder.OwnsMany(e => e.Prerequisites, p => p.ToTable("t_course_prerequisite"));
    }
}

public class CoursePrerequisiteConfiguration : IEntityTypeConfiguration<CoursePrerequisite>
{
    public void Configure(EntityTypeBuilder<CoursePrerequisite> builder)
    {
        builder.ToTable("t_course_prerequisite");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.RelatedCourseId).HasColumnName("related_course_id").IsRequired();
        builder.Property(e => e.RelationType).HasColumnName("relation_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.IsRequired).HasColumnName("is_required").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => new { e.CourseId, e.RelatedCourseId, e.RelationType }).IsUnique().HasDatabaseName("uk_course_related");
        builder.HasIndex(e => e.RelatedCourseId).HasDatabaseName("idx_related_course");
    }
}

public class SemesterConfiguration : IEntityTypeConfiguration<Semester>
{
    public void Configure(EntityTypeBuilder<Semester> builder)
    {
        builder.ToTable("t_semester");
        builder.HasKey(e => e.SemesterId);
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").ValueGeneratedOnAdd();
        builder.Property(e => e.SemesterName).HasColumnName("semester_name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.AcademicYear).HasColumnName("academic_year").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Term).HasColumnName("term").IsRequired();
        builder.Property(e => e.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(e => e.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(e => e.TeachStart).HasColumnName("teach_start").IsRequired();
        builder.Property(e => e.TeachEnd).HasColumnName("teach_end").IsRequired();
        builder.Property(e => e.TotalWeeks).HasColumnName("total_weeks").HasDefaultValue(18);
        builder.Property(e => e.IsCurrent).HasColumnName("is_current").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.SemesterName).IsUnique().HasDatabaseName("uk_semester_name");
        builder.HasIndex(e => e.IsCurrent).HasDatabaseName("idx_is_current");
    }
}

public class TeachingTaskConfiguration : IEntityTypeConfiguration<TeachingTask>
{
    public void Configure(EntityTypeBuilder<TeachingTask> builder)
    {
        builder.ToTable("t_teaching_task");
        builder.HasKey(e => e.TaskId);
        builder.Property(e => e.TaskId).HasColumnName("task_id").ValueGeneratedOnAdd();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.TeacherId).HasColumnName("teacher_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.ClassIds).HasColumnName("class_ids").HasColumnType("JSON").IsRequired();
        builder.Property(e => e.MaxStudents).HasColumnName("max_students");
        builder.Property(e => e.CurrentCount).HasColumnName("current_count").HasDefaultValue(0);
        builder.Property(e => e.WeeklyHours).HasColumnName("weekly_hours").HasPrecision(3, 1).IsRequired();
        builder.Property(e => e.TotalWeeks).HasColumnName("total_weeks").HasDefaultValue(18);
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => new { e.CourseId, e.SemesterId }).HasDatabaseName("idx_course_semester");
        builder.HasIndex(e => new { e.TeacherId, e.SemesterId }).HasDatabaseName("idx_teacher_semester");
        builder.HasIndex(e => e.SemesterId).HasDatabaseName("idx_semester_id");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ScheduleItemConfiguration : IEntityTypeConfiguration<ScheduleItem>
{
    public void Configure(EntityTypeBuilder<ScheduleItem> builder)
    {
        builder.ToTable("t_schedule_item");
        builder.HasKey(e => e.ItemId);
        builder.Property(e => e.ItemId).HasColumnName("item_id").ValueGeneratedOnAdd();
        builder.Property(e => e.TaskId).HasColumnName("task_id").IsRequired();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.TeacherId).HasColumnName("teacher_id").IsRequired();
        builder.Property(e => e.ClassroomId).HasColumnName("classroom_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.DayOfWeek).HasColumnName("day_of_week").IsRequired();
        builder.Property(e => e.StartPeriod).HasColumnName("start_period").IsRequired();
        builder.Property(e => e.EndPeriod).HasColumnName("end_period").IsRequired();
        builder.Property(e => e.StartWeek).HasColumnName("start_week").IsRequired();
        builder.Property(e => e.EndWeek).HasColumnName("end_week").IsRequired();
        builder.Property(e => e.WeekType).HasColumnName("week_type").HasMaxLength(10).HasDefaultValue("每周");
        builder.Property(e => e.IsLocked).HasColumnName("is_locked").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.TaskId).HasDatabaseName("idx_task_id");
        builder.HasIndex(e => new { e.ClassroomId, e.SemesterId }).HasDatabaseName("idx_classroom_semester");
        builder.HasIndex(e => new { e.TeacherId, e.SemesterId }).HasDatabaseName("idx_teacher_semester");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class SelectionRoundConfiguration : IEntityTypeConfiguration<SelectionRound>
{
    public void Configure(EntityTypeBuilder<SelectionRound> builder)
    {
        builder.ToTable("t_selection_round");
        builder.HasKey(e => e.RoundId);
        builder.Property(e => e.RoundId).HasColumnName("round_id").ValueGeneratedOnAdd();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.RoundType).HasColumnName("round_type").HasConversion<int>().IsRequired();
        builder.Property(e => e.RoundName).HasColumnName("round_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(e => e.EndTime).HasColumnName("end_time").IsRequired();
        builder.Property(e => e.Rules).HasColumnName("rules").HasColumnType("JSON").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(10).HasDefaultValue("未开始");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.SemesterId).HasDatabaseName("idx_semester_id");
        builder.HasIndex(e => e.Status).HasDatabaseName("idx_status");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class SelectionRecordConfiguration : IEntityTypeConfiguration<SelectionRecord>
{
    public void Configure(EntityTypeBuilder<SelectionRecord> builder)
    {
        builder.ToTable("t_selection_record");
        builder.HasKey(e => e.RecordId);
        builder.Property(e => e.RecordId).HasColumnName("record_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.TaskId).HasColumnName("task_id").IsRequired();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.RoundId).HasColumnName("round_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(10).HasDefaultValue("已选");
        builder.Property(e => e.IsLottery).HasColumnName("is_lottery").HasDefaultValue(false);
        builder.Property(e => e.SelectedAt).HasColumnName("selected_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.DroppedAt).HasColumnName("dropped_at");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => new { e.StudentId, e.TaskId }).IsUnique().HasDatabaseName("uk_student_task");
        builder.HasIndex(e => e.TaskId).HasDatabaseName("idx_task_id");
        builder.HasIndex(e => new { e.SemesterId, e.StudentId }).HasDatabaseName("idx_semester_student");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ScoreRecordConfiguration : IEntityTypeConfiguration<ScoreRecord>
{
    public void Configure(EntityTypeBuilder<ScoreRecord> builder)
    {
        builder.ToTable("t_score_record");
        builder.HasKey(e => e.RecordId);
        builder.Property(e => e.RecordId).HasColumnName("record_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.TaskId).HasColumnName("task_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.RegularScore).HasColumnName("regular_score").HasPrecision(5, 1);
        builder.Property(e => e.ExperimentScore).HasColumnName("experiment_score").HasPrecision(5, 1);
        builder.Property(e => e.FinalScore).HasColumnName("final_score").HasPrecision(5, 1);
        builder.Property(e => e.MidScore).HasColumnName("mid_score").HasPrecision(5, 1);
        builder.Property(e => e.TotalScore).HasColumnName("total_score").HasPrecision(5, 1);
        builder.Property(e => e.Grade).HasColumnName("grade").HasMaxLength(10);
        builder.Property(e => e.Gpa).HasColumnName("gpa").HasPrecision(3, 2);
        builder.Property(e => e.Credit).HasColumnName("credit").HasPrecision(3, 1).IsRequired();
        builder.Property(e => e.IsPassed).HasColumnName("is_passed");
        builder.Property(e => e.ScoreType).HasColumnName("score_type").HasMaxLength(10).HasDefaultValue("百分制");
        builder.Property(e => e.IsPublished).HasColumnName("is_published").HasDefaultValue(false);
        builder.Property(e => e.EnteredBy).HasColumnName("entered_by");
        builder.Property(e => e.EnteredAt).HasColumnName("entered_at");
        builder.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
        builder.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
        builder.Property(e => e.Remark).HasColumnName("remark").HasMaxLength(500);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => new { e.StudentId, e.TaskId }).IsUnique().HasDatabaseName("uk_student_task");
        builder.HasIndex(e => new { e.CourseId, e.SemesterId }).HasDatabaseName("idx_course_semester");
        builder.HasIndex(e => e.IsPublished).HasDatabaseName("idx_is_published");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ScoreRuleConfiguration : IEntityTypeConfiguration<ScoreRule>
{
    public void Configure(EntityTypeBuilder<ScoreRule> builder)
    {
        builder.ToTable("t_score_rule");
        builder.HasKey(e => e.RuleId);
        builder.Property(e => e.RuleId).HasColumnName("rule_id").ValueGeneratedOnAdd();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.RegularWeight).HasColumnName("regular_weight").HasPrecision(3, 2).HasDefaultValue(0.30);
        builder.Property(e => e.ExperimentWeight).HasColumnName("experiment_weight").HasPrecision(3, 2).HasDefaultValue(0.00);
        builder.Property(e => e.MidWeight).HasColumnName("mid_weight").HasPrecision(3, 2).HasDefaultValue(0.00);
        builder.Property(e => e.FinalWeight).HasColumnName("final_weight").HasPrecision(3, 2).HasDefaultValue(0.70);
        builder.Property(e => e.ScoreType).HasColumnName("score_type").HasMaxLength(10).HasDefaultValue("百分制");
        builder.Property(e => e.GpaMapping).HasColumnName("gpa_mapping").HasColumnType("JSON");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.CourseId).IsUnique().HasDatabaseName("uk_course_id");
    }
}

public class GpaSummaryConfiguration : IEntityTypeConfiguration<GpaSummary>
{
    public void Configure(EntityTypeBuilder<GpaSummary> builder)
    {
        builder.ToTable("t_gpa_summary");
        builder.HasKey(e => e.SummaryId);
        builder.Property(e => e.SummaryId).HasColumnName("summary_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id");
        builder.Property(e => e.TotalCredits).HasColumnName("total_credits").HasPrecision(5, 1).HasDefaultValue(0);
        builder.Property(e => e.TotalPoints).HasColumnName("total_points").HasPrecision(7, 2).HasDefaultValue(0);
        builder.Property(e => e.Gpa).HasColumnName("gpa").HasPrecision(3, 2).HasDefaultValue(0);
        builder.Property(e => e.MajorRank).HasColumnName("major_rank");
        builder.Property(e => e.MajorTotal).HasColumnName("major_total");
        builder.Property(e => e.ClassRank).HasColumnName("class_rank");
        builder.Property(e => e.ClassTotal).HasColumnName("class_total");
        builder.Property(e => e.CalculatedAt).HasColumnName("calculated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => new { e.StudentId, e.SemesterId }).IsUnique().HasDatabaseName("uk_student_semester");
        builder.HasIndex(e => new { e.SemesterId, e.Gpa }).HasDatabaseName("idx_semester_gpa");
    }
}

public class ExamArrangementConfiguration : IEntityTypeConfiguration<ExamArrangement>
{
    public void Configure(EntityTypeBuilder<ExamArrangement> builder)
    {
        builder.ToTable("t_exam_arrangement");
        builder.HasKey(e => e.ArrangementId);
        builder.Property(e => e.ArrangementId).HasColumnName("arrangement_id").ValueGeneratedOnAdd();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.TaskId).HasColumnName("task_id").IsRequired();
        builder.Property(e => e.SemesterId).HasColumnName("semester_id").IsRequired();
        builder.Property(e => e.ExamType).HasColumnName("exam_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.ExamDate).HasColumnName("exam_date").IsRequired();
        builder.Property(e => e.StartTime).HasColumnName("start_time").IsRequired();
        builder.Property(e => e.EndTime).HasColumnName("end_time").IsRequired();
        builder.Property(e => e.ClassroomId).HasColumnName("classroom_id").IsRequired();
        builder.Property(e => e.ExamForm).HasColumnName("exam_form").HasMaxLength(20).HasDefaultValue("笔试");
        builder.Property(e => e.MaxStudents).HasColumnName("max_students");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(10).HasDefaultValue("草稿");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => new { e.CourseId, e.SemesterId }).HasDatabaseName("idx_course_semester");
        builder.HasIndex(e => new { e.ClassroomId, e.ExamDate }).HasDatabaseName("idx_classroom_date");
        builder.HasIndex(e => e.ExamDate).HasDatabaseName("idx_exam_date");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ExamSeatConfiguration : IEntityTypeConfiguration<ExamSeat>
{
    public void Configure(EntityTypeBuilder<ExamSeat> builder)
    {
        builder.ToTable("t_exam_seat");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.ArrangementId).HasColumnName("arrangement_id").IsRequired();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.SeatNo).HasColumnName("seat_no").IsRequired();
        builder.Property(e => e.ExamNo).HasColumnName("exam_no").HasMaxLength(20);
        builder.Property(e => e.CheckInTime).HasColumnName("check_in_time");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => new { e.ArrangementId, e.StudentId }).IsUnique().HasDatabaseName("uk_arrangement_student");
        builder.HasIndex(e => new { e.ArrangementId, e.SeatNo }).IsUnique().HasDatabaseName("uk_arrangement_seat");
    }
}

public class InvigilationConfiguration : IEntityTypeConfiguration<Invigilation>
{
    public void Configure(EntityTypeBuilder<Invigilation> builder)
    {
        builder.ToTable("t_invigilation");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.ArrangementId).HasColumnName("arrangement_id").IsRequired();
        builder.Property(e => e.TeacherId).HasColumnName("teacher_id").IsRequired();
        builder.Property(e => e.Role).HasColumnName("role").HasMaxLength(20).HasDefaultValue("主监考");
        builder.Property(e => e.CheckInTime).HasColumnName("check_in_time");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(10).HasDefaultValue("已指派");
        builder.Property(e => e.FeeAmount).HasColumnName("fee_amount").HasPrecision(8, 2);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => new { e.ArrangementId, e.TeacherId }).IsUnique().HasDatabaseName("uk_arrangement_teacher");
        builder.HasIndex(e => e.TeacherId).HasDatabaseName("idx_teacher_id");
    }
}

public class DeferredExamConfiguration : IEntityTypeConfiguration<DeferredExam>
{
    public void Configure(EntityTypeBuilder<DeferredExam> builder)
    {
        builder.ToTable("t_deferred_exam");
        builder.HasKey(e => e.DeferredId);
        builder.Property(e => e.DeferredId).HasColumnName("deferred_id").ValueGeneratedOnAdd();
        builder.Property(e => e.StudentId).HasColumnName("student_id").IsRequired();
        builder.Property(e => e.ArrangementId).HasColumnName("arrangement_id").IsRequired();
        builder.Property(e => e.DeferType).HasColumnName("defer_type").HasMaxLength(10).HasDefaultValue("缓考");
        builder.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(e => e.ProofUrl).HasColumnName("proof_url").HasMaxLength(500);
        builder.Property(e => e.ApplyStatus).HasColumnName("apply_status").HasConversion<int>().HasDefaultValue(ApplyStatus.Pending);
        builder.Property(e => e.ApprovedBy).HasColumnName("approved_by");
        builder.Property(e => e.ApprovedAt).HasColumnName("approved_at");
        builder.Property(e => e.NewArrangementId).HasColumnName("new_arrangement_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.StudentId).HasDatabaseName("idx_student_id");
        builder.HasIndex(e => e.ApplyStatus).HasDatabaseName("idx_apply_status");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class BuildingConfiguration : IEntityTypeConfiguration<Building>
{
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.ToTable("t_building");
        builder.HasKey(e => e.BuildingId);
        builder.Property(e => e.BuildingId).HasColumnName("building_id").ValueGeneratedOnAdd();
        builder.Property(e => e.BuildingCode).HasColumnName("building_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.BuildingName).HasColumnName("building_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.CampusId).HasColumnName("campus_id").IsRequired();
        builder.Property(e => e.CampusName).HasColumnName("campus_name").HasMaxLength(50);
        builder.Property(e => e.TotalFloors).HasColumnName("total_floors");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => e.BuildingCode).IsUnique().HasDatabaseName("uk_building_code");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ClassroomConfiguration : IEntityTypeConfiguration<Classroom>
{
    public void Configure(EntityTypeBuilder<Classroom> builder)
    {
        builder.ToTable("t_classroom");
        builder.HasKey(e => e.ClassroomId);
        builder.Property(e => e.ClassroomId).HasColumnName("classroom_id").ValueGeneratedOnAdd();
        builder.Property(e => e.BuildingId).HasColumnName("building_id").IsRequired();
        builder.Property(e => e.RoomNo).HasColumnName("room_no").HasMaxLength(20).IsRequired();
        builder.Property(e => e.RoomName).HasColumnName("room_name").HasMaxLength(50).IsRequired();
        builder.Property(e => e.RoomType).HasColumnName("room_type").HasMaxLength(20).HasDefaultValue("普通教室");
        builder.Property(e => e.Capacity).HasColumnName("capacity").HasDefaultValue(0);
        builder.Property(e => e.FloorNo).HasColumnName("floor_no").HasDefaultValue(1);
        builder.Property(e => e.AreaSqm).HasColumnName("area_sqm").HasPrecision(6, 2);
        builder.Property(e => e.Equipment).HasColumnName("equipment").HasColumnType("JSON");
        builder.Property(e => e.HasMultimedia).HasColumnName("has_multimedia").HasDefaultValue(false);
        builder.Property(e => e.HasComputer).HasColumnName("has_computer").HasDefaultValue(false);
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(10).HasDefaultValue("正常");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => new { e.BuildingId, e.RoomNo }).IsUnique().HasDatabaseName("uk_building_room");
        builder.HasIndex(e => e.RoomType).HasDatabaseName("idx_room_type");
        builder.HasIndex(e => e.Capacity).HasDatabaseName("idx_capacity");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class ClassroomBookingConfiguration : IEntityTypeConfiguration<ClassroomBooking>
{
    public void Configure(EntityTypeBuilder<ClassroomBooking> builder)
    {
        builder.ToTable("t_classroom_booking");
        builder.HasKey(e => e.BookingId);
        builder.Property(e => e.BookingId).HasColumnName("booking_id").ValueGeneratedOnAdd();
        builder.Property(e => e.ClassroomId).HasColumnName("classroom_id").IsRequired();
        builder.Property(e => e.ApplicantId).HasColumnName("applicant_id").IsRequired();
        builder.Property(e => e.BookingDate).HasColumnName("booking_date").IsRequired();
        builder.Property(e => e.StartPeriod).HasColumnName("start_period").IsRequired();
        builder.Property(e => e.EndPeriod).HasColumnName("end_period").IsRequired();
        builder.Property(e => e.Purpose).HasColumnName("purpose").HasMaxLength(500).IsRequired();
        builder.Property(e => e.AttendeeCount).HasColumnName("attendee_count");
        builder.Property(e => e.ApplyStatus).HasColumnName("apply_status").HasConversion<int>().HasDefaultValue(ApplyStatus.Pending);
        builder.Property(e => e.ApprovedBy).HasColumnName("approved_by");
        builder.Property(e => e.ApprovedAt).HasColumnName("approved_at");
        builder.Property(e => e.RejectReason).HasColumnName("reject_reason").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => new { e.ClassroomId, e.BookingDate }).HasDatabaseName("idx_classroom_date");
        builder.HasIndex(e => e.ApplicantId).HasDatabaseName("idx_applicant_id");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class CollegeConfiguration : IEntityTypeConfiguration<College>
{
    public void Configure(EntityTypeBuilder<College> builder)
    {
        builder.ToTable("t_college");
        builder.HasKey(e => e.CollegeId);
        builder.Property(e => e.CollegeId).HasColumnName("college_id").ValueGeneratedOnAdd();
        builder.Property(e => e.CollegeCode).HasColumnName("college_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.CollegeName).HasColumnName("college_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.ShortName).HasColumnName("short_name").HasMaxLength(30);
        builder.Property(e => e.SortOrder).HasColumnName("sort_order").HasDefaultValue(0);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => e.CollegeCode).IsUnique().HasDatabaseName("uk_college_code");
    }
}

public class MajorConfiguration : IEntityTypeConfiguration<Major>
{
    public void Configure(EntityTypeBuilder<Major> builder)
    {
        builder.ToTable("t_major");
        builder.HasKey(e => e.MajorId);
        builder.Property(e => e.MajorId).HasColumnName("major_id").ValueGeneratedOnAdd();
        builder.Property(e => e.MajorCode).HasColumnName("major_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.MajorName).HasColumnName("major_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.CollegeId).HasColumnName("college_id").IsRequired();
        builder.Property(e => e.EduLevel).HasColumnName("edu_level").HasMaxLength(20).HasDefaultValue("本科");
        builder.Property(e => e.LengthOfSchool).HasColumnName("length_of_school").HasDefaultValue(4);
        builder.Property(e => e.DegreeType).HasColumnName("degree_type").HasMaxLength(30);
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => e.MajorCode).IsUnique().HasDatabaseName("uk_major_code");
        builder.HasIndex(e => e.CollegeId).HasDatabaseName("idx_college_id");
    }
}

public class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("t_class");
        builder.HasKey(e => e.ClassId);
        builder.Property(e => e.ClassId).HasColumnName("class_id").ValueGeneratedOnAdd();
        builder.Property(e => e.ClassName).HasColumnName("class_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.ClassCode).HasColumnName("class_code").HasMaxLength(30).IsRequired();
        builder.Property(e => e.MajorId).HasColumnName("major_id").IsRequired();
        builder.Property(e => e.GradeYear).HasColumnName("grade_year").IsRequired();
        builder.Property(e => e.CollegeId).HasColumnName("college_id").IsRequired();
        builder.Property(e => e.CounselorId).HasColumnName("counselor_id");
        builder.Property(e => e.HeadTeacherId).HasColumnName("head_teacher_id");
        builder.Property(e => e.MaxStudents).HasColumnName("max_students");
        builder.Property(e => e.CurrentCount).HasColumnName("current_count").HasDefaultValue(0);
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => e.ClassCode).IsUnique().HasDatabaseName("uk_class_code");
        builder.HasIndex(e => e.MajorId).HasDatabaseName("idx_major_id");
        builder.HasIndex(e => e.CollegeId).HasDatabaseName("idx_college_id");
    }
}

public class TrainingPlanConfiguration : IEntityTypeConfiguration<TrainingPlan>
{
    public void Configure(EntityTypeBuilder<TrainingPlan> builder)
    {
        builder.ToTable("t_training_plan");
        builder.HasKey(e => e.PlanId);
        builder.Property(e => e.PlanId).HasColumnName("plan_id").ValueGeneratedOnAdd();
        builder.Property(e => e.MajorId).HasColumnName("major_id").IsRequired();
        builder.Property(e => e.GradeYear).HasColumnName("grade_year").IsRequired();
        builder.Property(e => e.PlanName).HasColumnName("plan_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.TotalCredits).HasColumnName("total_credits").HasPrecision(5, 1).IsRequired();
        builder.Property(e => e.LengthOfSchool).HasColumnName("length_of_school").HasDefaultValue(4);
        builder.Property(e => e.PlanStructure).HasColumnName("plan_structure").HasColumnType("JSON").IsRequired();
        builder.Property(e => e.Version).HasColumnName("version").HasDefaultValue(1);
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.ApprovedBy).HasColumnName("approved_by");
        builder.Property(e => e.ApprovedAt).HasColumnName("approved_at");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => new { e.MajorId, e.GradeYear }).HasDatabaseName("idx_major_grade");
        builder.Ignore(e => e.DomainEvents);
    }
}

public class PlanCourseConfiguration : IEntityTypeConfiguration<PlanCourse>
{
    public void Configure(EntityTypeBuilder<PlanCourse> builder)
    {
        builder.ToTable("t_plan_course");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(e => e.PlanId).HasColumnName("plan_id").IsRequired();
        builder.Property(e => e.CourseId).HasColumnName("course_id").IsRequired();
        builder.Property(e => e.SemesterNo).HasColumnName("semester_no").IsRequired();
        builder.Property(e => e.CreditRequired).HasColumnName("credit_required").HasPrecision(3, 1).IsRequired();
        builder.Property(e => e.IsCompulsory).HasColumnName("is_compulsory").HasDefaultValue(true);
        builder.Property(e => e.Platform).HasColumnName("platform").HasMaxLength(50).HasDefaultValue("专业教育");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.HasIndex(e => new { e.PlanId, e.CourseId }).IsUnique().HasDatabaseName("uk_plan_course");
    }
}

public class ScheduleAdjustmentConfiguration : IEntityTypeConfiguration<ScheduleAdjustment>
{
    public void Configure(EntityTypeBuilder<ScheduleAdjustment> builder)
    {
        builder.ToTable("t_schedule_adjustment");
        builder.HasKey(e => e.AdjustId);
        builder.Property(e => e.AdjustId).HasColumnName("adjust_id").ValueGeneratedOnAdd();
        builder.Property(e => e.OriginalItemId).HasColumnName("original_item_id").IsRequired();
        builder.Property(e => e.AdjustType).HasColumnName("adjust_type").HasConversion<int>().IsRequired();
        builder.Property(e => e.OriginalDate).HasColumnName("original_date").IsRequired();
        builder.Property(e => e.OriginalPeriod).HasColumnName("original_period").HasMaxLength(20);
        builder.Property(e => e.NewDate).HasColumnName("new_date");
        builder.Property(e => e.NewPeriod).HasColumnName("new_period").HasMaxLength(20);
        builder.Property(e => e.NewClassroomId).HasColumnName("new_classroom_id");
        builder.Property(e => e.Reason).HasColumnName("reason").HasMaxLength(500).IsRequired();
        builder.Property(e => e.ApplyStatus).HasColumnName("apply_status").HasConversion<int>().HasDefaultValue(ApplyStatus.Pending);
        builder.Property(e => e.ApprovedBy).HasColumnName("approved_by");
        builder.Property(e => e.ApprovedAt).HasColumnName("approved_at");
        builder.Property(e => e.NotifyStatus).HasColumnName("notify_status").HasDefaultValue(false);
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
        builder.HasIndex(e => e.OriginalItemId).HasDatabaseName("idx_original_item");
        builder.HasIndex(e => e.ApplyStatus).HasDatabaseName("idx_apply_status");
        builder.Ignore(e => e.DomainEvents);
    }
}