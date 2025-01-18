using BuildingBlocks.Application;
using Microsoft.AspNetCore.Authorization;
using Modules.UserAcess.Application.Authorization.GetUserPermissions;
using Modules.UserAcess.Application.Contracts;

namespace API.Configuration.Authorization;

public class HasPermissionAuthorizationRequirement : IAuthorizationRequirement
{
}