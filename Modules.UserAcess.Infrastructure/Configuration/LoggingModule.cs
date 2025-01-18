using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Modules.UserAcess.Infrastructure.Configuration;

internal class LoggingModule
{
    public static void Register(IServiceCollection services, ILogger logger) => 
        services.AddSingleton(logger);
}