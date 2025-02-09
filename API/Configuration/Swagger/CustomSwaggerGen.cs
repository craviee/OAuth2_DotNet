using Microsoft.OpenApi.Models;

namespace API.Configuration.Swagger;

public static class CustomSwaggerGen
{
    public static void AddCustomSwaggerGen(this IServiceCollection services, string apiVersionName)
    {
        services.AddSwaggerGen(options =>
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
    }
}