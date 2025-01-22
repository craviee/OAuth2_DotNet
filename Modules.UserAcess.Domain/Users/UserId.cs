using BuildingBlocks.Domain;

namespace Modules.UserAcess.Domain.Users;

public class UserId : TypedIdValueBase
{
    public UserId(Guid value)
        : base(value)
    {
    }
}