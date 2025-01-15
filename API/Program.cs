// using CompanyName.MyMeetings.API.Configuration.Authorization;
// using CompanyName.MyMeetings.API.Configuration.ExecutionContext;
// using CompanyName.MyMeetings.API.Configuration.Extensions;
// using CompanyName.MyMeetings.API.Configuration.Validation;
// using CompanyName.MyMeetings.BuildingBlocks.Application;
// using CompanyName.MyMeetings.BuildingBlocks.Domain;
// using CompanyName.MyMeetings.BuildingBlocks.Infrastructure.Emails;
// using CompanyName.MyMeetings.Modules.Administration.Infrastructure.Configuration;
// using CompanyName.MyMeetings.Modules.Meetings.Infrastructure.Configuration;
// using CompanyName.MyMeetings.Modules.Payments.Infrastructure.Configuration;
// using CompanyName.MyMeetings.Modules.Registrations.Infrastructure.Configuration;
// using CompanyName.MyMeetings.Modules.UserAccess.Infrastructure.Configuration;
// using CompanyName.MyMeetings.Modules.UserAccess.Infrastructure.Configuration.Identity;
// using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
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
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyMeetings API",
        Version = "v1",
        Description = "An API for managing meetings, payments, and user access."
    });
});

// // Add custom services
// services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// services.AddSingleton<IExecutionContextAccessor, ExecutionContextAccessor>();
//
// // Add ProblemDetails middleware
// services.AddProblemDetails(options =>
// {
//     options.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));
//     options.Map<BusinessRuleValidationException>(ex => new BusinessRuleValidationExceptionProblemDetails(ex));
// });
//
// // Configure Authorization
// services.AddAuthorization(options =>
// {
//     options.AddPolicy(HasPermissionAttribute.HasPermissionPolicyName, policyBuilder =>
//     {
//         policyBuilder.Requirements.Add(new HasPermissionAuthorizationRequirement());
//         policyBuilder.AddAuthenticationSchemes("Bearer");
//     });
// });
// services.AddScoped<IAuthorizationHandler, HasPermissionAuthorizationHandler>();
//
// // Initialize modules (using reflection or direct DI)
// InitializeModules(services, configuration);
//
// var app = builder.Build();
//
// // Configure middleware pipeline
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     app.UseProblemDetails(); // Only enable detailed problem descriptions in development
// }
// else
// {
//     app.UseHsts();
// }
//
// app.UseHttpsRedirection();
// app.UseRouting();
// app.UseAuthorization();
//
// // Add a custom middleware for correlation
// app.UseMiddleware<CorrelationMiddleware>();
//
// // Map controllers
// app.MapControllers();
//
// app.Run();
//
// // Helper method to initialize modules
// void InitializeModules(IServiceCollection services, IConfiguration configuration)
// {
//     var executionContextAccessor = new ExecutionContextAccessor(
//         services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>());
//
//     var emailsConfiguration = new EmailsConfiguration(configuration["EmailsConfiguration:FromEmail"]);
//
//     var connectionString = configuration.GetConnectionString("MeetingsConnectionString");
//
//     MeetingsStartup.Initialize(
//         connectionString,
//         executionContextAccessor,
//         Log.Logger,
//         emailsConfiguration,
//         null);
//
//     AdministrationStartup.Initialize(
//         connectionString,
//         executionContextAccessor,
//         Log.Logger,
//         null);
//
//     UserAccessStartup.Initialize(
//         connectionString,
//         executionContextAccessor,
//         Log.Logger,
//         emailsConfiguration,
//         configuration["Security:TextEncryptionKey"],
//         null,
//         null);
//
//     PaymentsStartup.Initialize(
//         connectionString,
//         executionContextAccessor,
//         Log.Logger,
//         emailsConfiguration,
//         null);
//
//     RegistrationsStartup.Initialize(
//         connectionString,
//         executionContextAccessor,
//         Log.Logger,
//         emailsConfiguration,
//         configuration["Security:TextEncryptionKey"],
//         null,
//         null);
// }