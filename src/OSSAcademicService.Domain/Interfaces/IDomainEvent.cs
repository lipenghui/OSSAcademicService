using MediatR;

namespace OSSAcademicService.Domain.Interfaces;

/// <summary>
/// 领域事件接口，继承 MediatR INotification
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}