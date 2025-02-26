using System.Text;
using API.Configuration.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Configuration.Authentication;

public static class CustomAuthentication
{
    public static void AddCustomAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
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