using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string apiVersionName = "v1";
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(apiVersionName, new OpenApiInfo
    {
        Title = "OAuth Sample API",
        Version = apiVersionName,
        Description = "Simple API using OAuth",
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your valid JWT token.\n\nExample: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            }, []
        }
    });
});

// Add DbContext for Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSqlServerConnection")));

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "app",
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey("YourSuperSecretKeyThatIsAtLeast32CharactersLong"u8.ToArray()),
        ClockSkew = TimeSpan.Zero,
        LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
        {
            if ((securityToken as JsonWebToken)!.Typ == "JWT")
            {
                if (securityToken is JsonWebToken token)
                {
                    DateTime now = DateTime.UtcNow;
                    DateTime iat = token.IssuedAt.ToUniversalTime();
                    if(now >= iat.AddMinutes(1))
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

WebApplication app = builder.Build();

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"/swagger/{apiVersionName}/swagger.json", apiVersionName);
    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();