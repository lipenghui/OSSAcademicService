namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 监考安排
/// </summary>
public class Invigilation
{
    public long Id { get; private set; }
    public long ArrangementId { get; private set; }
    public long TeacherId { get; private set; }
    public string Role { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    public string Status { get; private set; }
    public decimal? FeeAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Invigilation(long arrangementId, long teacherId, string role = "主监考")
    {
        ArrangementId = arrangementId;
        TeacherId = teacherId;
        Role = role;
        Status = "已指派";
        CreatedAt = DateTime.UtcNow;
    }

    private Invigilation() { }

    public void Confirm()
    {
        Status = "已确认";
    }

    public void CheckIn()
    {
        Status = "已签到";
        CheckInTime = DateTime.UtcNow;
    }

    public void SetFee(decimal amount)
    {
        FeeAmount = amount;
    }
}