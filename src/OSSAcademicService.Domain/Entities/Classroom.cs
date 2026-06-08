using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 教室聚合根
/// </summary>
public class Classroom : IAggregateRoot
{
    public long ClassroomId { get; private set; }
    public long BuildingId { get; private set; }
    public string RoomNo { get; private set; }
    public string RoomName { get; private set; }
    public string RoomType { get; private set; }
    public int Capacity { get; private set; }
    public int FloorNo { get; private set; }
    public decimal? AreaSqm { get; private set; }
    public string? Equipment { get; private set; }
    public bool HasMultimedia { get; private set; }
    public bool HasComputer { get; private set; }
    public string Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Classroom() { }

    public static Classroom Create(long buildingId, string roomNo, string roomName,
        string roomType, int capacity, int floorNo = 1)
    {
        return new Classroom
        {
            BuildingId = buildingId,
            RoomNo = roomNo,
            RoomName = roomName,
            RoomType = roomType,
            Capacity = capacity,
            FloorNo = floorNo,
            Status = "正常",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string roomName, string roomType, int capacity, string? equipment,
        bool hasMultimedia, bool hasComputer)
    {
        RoomName = roomName;
        RoomType = roomType;
        Capacity = capacity;
        Equipment = equipment;
        HasMultimedia = hasMultimedia;
        HasComputer = hasComputer;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetMaintenance()
    {
        Status = "维修中";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetNormal()
    {
        Status = "正常";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}