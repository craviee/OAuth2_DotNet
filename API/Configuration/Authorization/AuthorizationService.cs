using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace API.Configuration.Authorization;

public class AuthorizationService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
    : IAuthorizationService
{
    public async Task<bool> HasAuthorizationAsync(Roles minimumRequiredRole)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return false;

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return false;
        
        var userRoles = await userManager.GetRolesAsync(user);

        foreach (string role in userRoles)
        {
            if (Enum.TryParse(role, out Roles userRole))
            {
                if(userRole >= minimumRequiredRole)
                    return true;
            }
        }
        return false;
    }
}