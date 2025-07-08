using MediatR;

namespace VSADemo.Common.Domain.Base.Interfaces;

/// <summary>
/// Can be raised by an AggregateRoot to notify subscribers of a domain event.
/// </summary>
public interface IDomainEvent : INotification;