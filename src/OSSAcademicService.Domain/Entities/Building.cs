using OSSAcademicService.Domain.Interfaces;

namespace OSSAcademicService.Domain.Entities;

/// <summary>
/// 教学楼聚合根
/// </summary>
public class Building : IAggregateRoot
{
    public long BuildingId { get; private set; }
    public string BuildingCode { get; private set; }
    public string BuildingName { get; private set; }
    public long CampusId { get; private set; }
    public string? CampusName { get; private set; }
    public int? TotalFloors { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Building() { }

    public static Building Create(string buildingCode, string buildingName, long campusId, string? campusName = null, int? totalFloors = null)
    {
        return new Building
        {
            BuildingCode = buildingCode,
            BuildingName = buildingName,
            CampusId = campusId,
            CampusName = campusName,
            TotalFloors = totalFloors,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    private void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}