namespace OSSAcademicService.Application.DTOs;

// ===== Student DTOs =====

public class StudentProfileDto
{
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = "";
    public long UserId { get; set; }
    public long CollegeId { get; set; }
    public string CollegeName { get; set; } = "";
    public long MajorId { get; set; }
    public string MajorName { get; set; } = "";
    public long? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int GradeYear { get; set; }
    public string EduLevel { get; set; } = "";
    public int LengthOfSchool { get; set; }
    public string Status { get; set; } = "";
    public DateOnly? EnrollmentDate { get; set; }
    public DateOnly? ExpectedGraduation { get; set; }
    public DateOnly? ActualGraduation { get; set; }
}

public class StudentListDto
{
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = "";
    public long CollegeId { get; set; }
    public string CollegeName { get; set; } = "";
    public long MajorId { get; set; }
    public string MajorName { get; set; } = "";
    public long? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int GradeYear { get; set; }
    public string Status { get; set; } = "";
}

public class StatusChangeDto
{
    public long ChangeId { get; set; }
    public long StudentId { get; set; }
    public string ChangeType { get; set; } = "";
    public string ChangeReason { get; set; } = "";
    public string ApplyStatus { get; set; } = "";
    public long? AppliedBy { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GraduationAuditDto
{
    public long AuditId { get; set; }
    public long StudentId { get; set; }
    public long SemesterId { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal RequiredCredits { get; set; }
    public decimal? Gpa { get; set; }
    public string Result { get; set; } = "";
    public string? CertificateNo { get; set; }
    public string? DegreeCertNo { get; set; }
    public long? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

// ===== Course DTOs =====

public class CourseDetailDto
{
    public long CourseId { get; set; }
    public string CourseCode { get; set; } = "";
    public string CourseName { get; set; } = "";
    public string? CourseNameEn { get; set; }
    public decimal Credit { get; set; }
    public int TotalHours { get; set; }
    public int TeachingHours { get; set; }
    public int LabHours { get; set; }
    public int PracticeHours { get; set; }
    public string TeachingMode { get; set; } = "";
    public string CourseType { get; set; } = "";
    public long CollegeId { get; set; }
    public string? OutlineUrl { get; set; }
    public int? MaxCapacity { get; set; }
    public string AssessMethod { get; set; } = "";
    public bool IsActive { get; set; }
}

public class CourseListDto
{
    public long CourseId { get; set; }
    public string CourseCode { get; set; } = "";
    public string CourseName { get; set; } = "";
    public decimal Credit { get; set; }
    public string CourseType { get; set; } = "";
    public long CollegeId { get; set; }
}

public class CreateCourseDto
{
    public string CourseCode { get; set; } = "";
    public string CourseName { get; set; } = "";
    public string? CourseNameEn { get; set; }
    public decimal Credit { get; set; }
    public int TotalHours { get; set; }
    public int TeachingHours { get; set; }
    public int LabHours { get; set; }
    public int PracticeHours { get; set; }
    public string TeachingMode { get; set; } = "讲授";
    public string CourseType { get; set; } = "";
    public long CollegeId { get; set; }
    public int? MaxCapacity { get; set; }
    public string AssessMethod { get; set; } = "考试";
}

public class UpdateCourseDto
{
    public string CourseName { get; set; } = "";
    public string? CourseNameEn { get; set; }
    public decimal Credit { get; set; }
    public int TotalHours { get; set; }
    public int TeachingHours { get; set; }
    public int LabHours { get; set; }
    public int PracticeHours { get; set; }
    public string? OutlineUrl { get; set; }
}

// ===== Schedule DTOs =====

public class ScheduleItemDto
{
    public long ItemId { get; set; }
    public long TaskId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public long TeacherId { get; set; }
    public long ClassroomId { get; set; }
    public string ClassroomName { get; set; } = "";
    public long SemesterId { get; set; }
    public int DayOfWeek { get; set; }
    public int StartPeriod { get; set; }
    public int EndPeriod { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; }
    public string WeekType { get; set; } = "";
}

public class AddScheduleItemDto
{
    public long TaskId { get; set; }
    public long CourseId { get; set; }
    public long TeacherId { get; set; }
    public long ClassroomId { get; set; }
    public long SemesterId { get; set; }
    public int DayOfWeek { get; set; }
    public int StartPeriod { get; set; }
    public int EndPeriod { get; set; }
    public int StartWeek { get; set; }
    public int EndWeek { get; set; }
    public string WeekType { get; set; } = "每周";
}

// ===== Selection DTOs =====

public class SelectionRoundDto
{
    public long RoundId { get; set; }
    public long SemesterId { get; set; }
    public string RoundType { get; set; } = "";
    public string RoundName { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "";
}

public class CreateSelectionRoundDto
{
    public long SemesterId { get; set; }
    public string RoundType { get; set; } = "";
    public string RoundName { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Rules { get; set; } = "{}";
}

public class SelectionRecordDto
{
    public long RecordId { get; set; }
    public long StudentId { get; set; }
    public long TaskId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public long RoundId { get; set; }
    public long SemesterId { get; set; }
    public string Status { get; set; } = "";
    public DateTime SelectedAt { get; set; }
    public DateTime? DroppedAt { get; set; }
}

// ===== Score DTOs =====

public class ScoreRecordDto
{
    public long RecordId { get; set; }
    public long StudentId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public long TaskId { get; set; }
    public long SemesterId { get; set; }
    public decimal? RegularScore { get; set; }
    public decimal? ExperimentScore { get; set; }
    public decimal? MidScore { get; set; }
    public decimal? FinalScore { get; set; }
    public decimal? TotalScore { get; set; }
    public string? Grade { get; set; }
    public decimal? Gpa { get; set; }
    public decimal Credit { get; set; }
    public bool? IsPassed { get; set; }
    public bool IsPublished { get; set; }
}

public class EnterScoreDto
{
    public decimal? Regular { get; set; }
    public decimal? Experiment { get; set; }
    public decimal? Mid { get; set; }
    public decimal? Final { get; set; }
}

public class BatchScoreEntryDto
{
    public long StudentId { get; set; }
    public decimal? Regular { get; set; }
    public decimal? Experiment { get; set; }
    public decimal? Mid { get; set; }
    public decimal? Final { get; set; }
}

public class StudentTranscriptDto
{
    public long StudentId { get; set; }
    public string StudentNo { get; set; } = "";
    public List<SemesterTranscriptDto> Semesters { get; set; } = new();
    public decimal? CumulativeGpa { get; set; }
    public decimal TotalCredits { get; set; }
}

public class SemesterTranscriptDto
{
    public long SemesterId { get; set; }
    public string SemesterName { get; set; } = "";
    public List<TranscriptRecordDto> Records { get; set; } = new();
    public decimal? SemesterGpa { get; set; }
    public decimal SemesterCredits { get; set; }
}

public class TranscriptRecordDto
{
    public long CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public decimal Credit { get; set; }
    public decimal? TotalScore { get; set; }
    public decimal? Gpa { get; set; }
    public string? Grade { get; set; }
}

public class GpaSummaryDto
{
    public long StudentId { get; set; }
    public long? SemesterId { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal TotalPoints { get; set; }
    public decimal Gpa { get; set; }
    public int? MajorRank { get; set; }
    public int? ClassRank { get; set; }
}

public class ScoreAnalysisDto
{
    public long TaskId { get; set; }
    public int TotalStudents { get; set; }
    public int PublishedCount { get; set; }
    public decimal? AverageScore { get; set; }
    public decimal? MaxScore { get; set; }
    public decimal? MinScore { get; set; }
    public double? StandardDeviation { get; set; }
    public double? PassRate { get; set; }
    public ScoreDistributionDto? Distribution { get; set; }
}

public class ScoreDistributionDto
{
    public int Excellent { get; set; }
    public int Good { get; set; }
    public int Medium { get; set; }
    public int Pass { get; set; }
    public int Fail { get; set; }
}

// ===== Exam DTOs =====

public class ExamArrangementDto
{
    public long ArrangementId { get; set; }
    public long CourseId { get; set; }
    public string CourseName { get; set; } = "";
    public long TaskId { get; set; }
    public long SemesterId { get; set; }
    public string ExamType { get; set; } = "";
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public long ClassroomId { get; set; }
    public string ClassroomName { get; set; } = "";
    public string ExamForm { get; set; } = "";
    public string Status { get; set; } = "";
}

public class CreateExamDto
{
    public long SemesterId { get; set; }
    public long CourseId { get; set; }
    public long TaskId { get; set; }
    public string ExamType { get; set; } = "";
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public long ClassroomId { get; set; }
    public string ExamForm { get; set; } = "笔试";
}

public class ExamSeatDto
{
    public long Id { get; set; }
    public long ArrangementId { get; set; }
    public long StudentId { get; set; }
    public int SeatNo { get; set; }
    public string? ExamNo { get; set; }
    public DateTime? CheckInTime { get; set; }
}

// ===== Classroom DTOs =====

public class ClassroomDto
{
    public long ClassroomId { get; set; }
    public long BuildingId { get; set; }
    public string BuildingName { get; set; } = "";
    public string RoomNo { get; set; } = "";
    public string RoomName { get; set; } = "";
    public string RoomType { get; set; } = "";
    public int Capacity { get; set; }
    public string Status { get; set; } = "";
}

public class ClassroomDetailDto
{
    public long ClassroomId { get; set; }
    public long BuildingId { get; set; }
    public string BuildingName { get; set; } = "";
    public string RoomNo { get; set; } = "";
    public string RoomName { get; set; } = "";
    public string RoomType { get; set; } = "";
    public int Capacity { get; set; }
    public int FloorNo { get; set; }
    public bool HasMultimedia { get; set; }
    public bool HasComputer { get; set; }
    public string? Equipment { get; set; }
    public string Status { get; set; } = "";
}

// ===== Base Data DTOs =====

public class SemesterDto
{
    public long SemesterId { get; set; }
    public string SemesterName { get; set; } = "";
    public string AcademicYear { get; set; } = "";
    public int Term { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public bool IsCurrent { get; set; }
}

public class CollegeDto
{
    public long CollegeId { get; set; }
    public string CollegeCode { get; set; } = "";
    public string CollegeName { get; set; } = "";
    public string? ShortName { get; set; }
}

public class MajorDto
{
    public long MajorId { get; set; }
    public string MajorCode { get; set; } = "";
    public string MajorName { get; set; } = "";
    public long CollegeId { get; set; }
    public string EduLevel { get; set; } = "";
    public int LengthOfSchool { get; set; }
}

public class ClassDto
{
    public long ClassId { get; set; }
    public string ClassCode { get; set; } = "";
    public string ClassName { get; set; } = "";
    public long MajorId { get; set; }
    public int GradeYear { get; set; }
    public int CurrentCount { get; set; }
}

// ===== API Response =====

public class ApiResponse<T>
{
    public int Code { get; set; }
    public string Message { get; set; } = "ok";
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Code = 0, Data = data };
    public static ApiResponse<object> Fail(int code, string message) => new() { Code = code, Message = message, Data = null };
}