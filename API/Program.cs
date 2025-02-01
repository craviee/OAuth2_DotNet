using Hellang.Middleware.ProblemDetails;
using API.Configuration.Authorization;
using API.Configuration.ExecutionContext;
using API.Configuration.Validation;
using BuildingBlocks.Application;
using BuildingBlocks.Domain;
using Microsoft.AspNetCore.Authorization;
using Modules.UserAcess.Infrastructure.Configuration;
using Serilog;
using Serilog.Formatting.Compact;

// Set up logger
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Module}] [{Context}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new CompactJsonFormatter(), "logs/logs")
    .CreateLogger();

Log.Information("Logger configured");

var builder = WebApplication.CreateBuilder(args);

// Add Serilog as the logging provider
builder.Host.UseSerilog();

var configuration = builder.Configuration;

// Add configuration files and environment variables
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables("Meetings_");

// Set up services
var services = builder.Services;

// Register application services
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
// services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "MyMeetings API",
//         Version = "v1",
//         Description = "An API for managing meetings, payments, and user access."
//     });
// });

// Add custom services
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>();

// Add ProblemDetails middleware
services.AddProblemDetails(options =>
{
    options.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));
    options.Map<BusinessRuleValidationException>(ex => new BusinessRuleValidationExceptionProblemDetails(ex));
});

// Configure Authorization
services.AddAuthorization(options =>
{
    options.AddPolicy(HasPermissionAttribute.HasPermissionPolicyName, policyBuilder =>
    {
        policyBuilder.Requirements.Add(new HasPermissionAuthorizationRequirement());
        policyBuilder.AddAuthenticationSchemes("Bearer");
    });
});
services.AddScoped<IAuthorizationHandler, HasPermissionAuthorizationHandler>();

// Initialize modules (using reflection or direct DI)
InitializeModules(services, configuration);

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseProblemDetails(); // Only enable detailed problem descriptions in development
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Add a custom middleware for correlation
app.UseMiddleware<CorrelationMiddleware>();

// Map controllers
app.MapControllers();

app.Run();

// Helper method to initialize modules
void InitializeModules(IServiceCollection services, IConfiguration configuration)
{
    // var executionContextAccessor = new ExecutionContextAccessor(
    //     services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>());
    //
    // var connectionString = configuration.GetConnectionString("DbConnectionString");
    //
    // UserAccessStartup.Initialize(
    //     connectionString,
    //     executionContextAccessor,
    //     Log.Logger,
    //     configuration["Security:TextEncryptionKey"],
    //     null,
    //     null);
}