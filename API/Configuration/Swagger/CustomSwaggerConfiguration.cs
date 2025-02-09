using Microsoft.IdentityModel.Protocols.Configuration;

namespace API.Configuration.Swagger;

public static class CustomSwaggerConfiguration
{
    public static void UseCustomizedSwagger(this WebApplication app,
        ConfigurationManager configurationManager)
    {
        string apiVersionName = configurationManager["ApiVersion"] ?? throw new InvalidConfigurationException();
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "swagger/{documentName}/swagger.json";
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{apiVersionName}/swagger.json", apiVersionName);
            options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        });
    }
}