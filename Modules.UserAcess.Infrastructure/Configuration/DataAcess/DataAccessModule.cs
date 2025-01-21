using BuildingBlocks.Application.Data;
using BuildingBlocks.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;

namespace Modules.UserAcess.Infrastructure.Configuration.DataAcess;

internal class DataAccessModule
{
    public static void Register(ServiceCollection services, string connectionString, SerilogLoggerFactory loggerFactory)
    {
        
        services.AddScoped<ISqlConnectionFactory>(provider => new SqlConnectionFactory(connectionString));

        services.AddScoped(provider =>
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<UserAccessContext>();
            dbContextOptionsBuilder.UseSqlServer(connectionString);

            dbContextOptionsBuilder
                .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

            return new UserAccessContext(dbContextOptionsBuilder.Options, loggerFactory);
        });

        var infrastructureAssembly = typeof(UserAccessContext).Assembly;
        var repositoryTypes = infrastructureAssembly.GetTypes()
        .Where(type => type.Name.EndsWith("Repository") && type is { IsClass: true, IsAbstract: false });

        foreach (var repositoryType in repositoryTypes)
        {
            var implementedInterfaces = repositoryType.GetInterfaces();
            foreach (var implementedInterface in implementedInterfaces)
            {
                services.AddScoped(implementedInterface, repositoryType);
            }
        }
    }
}