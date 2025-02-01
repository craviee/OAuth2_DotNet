var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string apiVersionName = "v1";
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(apiVersionName, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OAuth Sample API",
        Version = apiVersionName,
        Description = "Simple API using OAuth",
    });
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

app.UseAuthorization();

app.MapControllers();

app.Run();