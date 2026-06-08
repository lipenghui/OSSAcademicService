namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 考试座位
/// </summary>
public class ExamSeat
{
    public long Id { get; private set; }
    public long ArrangementId { get; private set; }
    public long StudentId { get; private set; }
    public int SeatNo { get; private set; }
    public string? ExamNo { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ExamSeat(long arrangementId, long studentId, int seatNo, string? examNo = null)
    {
        ArrangementId = arrangementId;
        StudentId = studentId;
        SeatNo = seatNo;
        ExamNo = examNo;
        CreatedAt = DateTime.UtcNow;
    }

    private ExamSeat() { }

    public void CheckIn()
    {
        CheckInTime = DateTime.UtcNow;
    }
}