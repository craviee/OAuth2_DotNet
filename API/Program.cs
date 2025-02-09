using API.Configuration;
using API.Configuration.Swagger;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDependencyInjection(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseCustomizedSwagger(builder.Configuration);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();