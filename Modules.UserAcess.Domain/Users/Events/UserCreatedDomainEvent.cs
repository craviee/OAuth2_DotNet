using BuildingBlocks.Domain;

namespace Modules.UserAcess.Domain.Users.Events;

public class UserCreatedDomainEvent : DomainEventBase
{
    public UserCreatedDomainEvent(UserId id)
    {
        Id = id;
    }
    public new UserId Id { get; }
}