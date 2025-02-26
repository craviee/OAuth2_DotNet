using System.Text;
using API.Configuration.Authentication;
using API.Configuration.Authorization;
using API.Configuration.Entity;
using API.Configuration.Identity;
using API.Configuration.Settings;
using API.Configuration.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Configuration;

public static class Services
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();
        var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthorizationService, AuthorizationService>();
        services.AddCustomSwaggerGen(apiSettings?.ApiVersion ?? throw new InvalidConfigurationException());
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(databaseSettings?.DefaultSqlServerConnection ?? throw new InvalidConfigurationException()));
        services.AddCustomAuthentication(jwtSettings ?? throw new InvalidConfigurationException());
        services.AddCustomIdentity();
    }
}