using Microsoft.Extensions.DependencyInjection;

namespace Modules.UserAcess.Infrastructure.Configuration;

internal static class UserAccessCompositionRoot
{
    private static IServiceProvider _serviceProvider;

    internal static void SetServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal static IServiceScope  BeginLifetimeScope()
    {
        if (_serviceProvider == null)
        {
            throw new InvalidOperationException("Service provider has not been initialized. Call SetServiceProvider first.");
        }
        return _serviceProvider.CreateScope();
    }
}