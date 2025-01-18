using Modules.UserAcess.Application.Contracts;

namespace Modules.UserAcess.Application.Authorization.GetUserPermissions;

public class GetUserPermissionsQuery : QueryBase<List<UserPermissionDto>>
{
    public GetUserPermissionsQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}