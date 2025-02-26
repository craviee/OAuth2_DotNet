using API.Configuration.Settings;

namespace API.Configuration;

public static class Options
{
    public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
    }
}