using API.Configuration.Settings;

namespace API.Configuration;

public static class DependencyInjectionConfigurator
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
        services.Configure<ApiSettings>(configuration.GetSection("AppSettings"));
    }
}