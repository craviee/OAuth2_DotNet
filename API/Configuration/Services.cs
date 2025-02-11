using System.Text;
using API.Configuration.Entity;
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
        
        // services.AddCustomSwaggerGen(configurationManager["ApiVersion"] ?? throw new InvalidConfigurationException());
        services.AddCustomSwaggerGen(apiSettings?.ApiVersion ?? throw new InvalidConfigurationException());

        var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(databaseSettings?.DefaultSqlServerConnection ?? throw new InvalidConfigurationException()));

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
        });

        // Add Identity services
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // Configure JWT Authentication
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
                byte[] key = Encoding.UTF8.GetBytes(jwtSettings?.SecurityKey ??
                                                    throw new InvalidConfigurationException());
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.ValidIssuer ?? throw new InvalidConfigurationException(),
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    {
                        if ((securityToken as JsonWebToken)!.Typ == "JWT")
                        {
                            if (securityToken is JsonWebToken token)
                            {
                                DateTime now = DateTime.UtcNow;
                                DateTime iat = token.IssuedAt.ToUniversalTime();
                                if (now >= iat.AddMinutes(jwtSettings.TokenLifetimeMinutes ?? throw new InvalidConfigurationException()))
                                    throw new SecurityTokenExpiredException("The token has expired.");
                                return true;
                            }
                        }

                        return false;
                    }
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.Headers.Add("Token-Error", "expired");
                            context.Response.Headers.Add("Access-Control-Expose-Headers", "Token-Error");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var response = new
                            {
                                error = "invalid_token",
                                error_description = "The token is expired. Please obtain a new token."
                            };

                            return context.Response.WriteAsJsonAsync(response);
                        }

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var genericResponse = new
                        {
                            error = "invalid_token",
                            error_description = "The token is invalid or has failed validation."
                        };

                        return context.Response.WriteAsJsonAsync(genericResponse);
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine("Authentication challenge: " + context.Error);
                        return Task.CompletedTask;
                    }
                };
            });
    }
}