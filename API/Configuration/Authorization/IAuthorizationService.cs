namespace API.Configuration.Authorization;

public interface IAuthorizationService
{
    Task<bool> HasAuthorizationAsync(Roles minimumRequiredRole);
}