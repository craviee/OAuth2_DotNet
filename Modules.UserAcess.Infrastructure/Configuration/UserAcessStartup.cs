using System.ComponentModel;
using BuildingBlocks.Application;
using BuildingBlocks.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Modules.UserAcess.Infrastructure.Configuration.DataAcess;
using Modules.UserAcess.Infrastructure.Configuration.EventBus;
using Modules.UserAcess.Infrastructure.Configuration.Quartz;
using Serilog;

namespace Modules.UserAcess.Infrastructure.Configuration;

public class UserAccessStartup
{
    private static IServiceProvider _serviceProvider;

    public static void Initialize(
        string connectionString,
        IExecutionContextAccessor executionContextAccessor,
        ILogger logger,
        string textEncryptionKey,
        IEventsBus eventsBus,
        long? internalProcessingPoolingInterval = null)
    {
        var moduleLogger = logger.ForContext("Module", "UserAccess");

        ConfigureCompositionRoot(
            connectionString,
            executionContextAccessor,
            logger,
            textEncryptionKey,
            eventsBus);

        QuartzStartup.Initialize(moduleLogger, internalProcessingPoolingInterval);

        EventsBusStartup.Initialize(moduleLogger);
    }

    private static void ConfigureCompositionRoot(
        string connectionString,
        IExecutionContextAccessor executionContextAccessor,
        ILogger logger,
        string textEncryptionKey,
        IEventsBus eventsBus)
    {
        var serviceCollection = new ServiceCollection();

        //serviceCollection.RegisterModule(new LoggingModule(logger.ForContext("Module", "UserAccess")));
        LoggingModule.Register(serviceCollection, logger.ForContext("Module", "UserAccess"));

        var loggerFactory = new Serilog.Extensions.Logging.SerilogLoggerFactory(logger);

        DataAccessModule.Register(serviceCollection, connectionString, loggerFactory);
        // serviceCollection.RegisterModule(new DataAccessModule(connectionString, loggerFactory));
        serviceCollection.RegisterModule(new ProcessingModule());
        serviceCollection.RegisterModule(new EventsBusModule(eventsBus));
        serviceCollection.RegisterModule(new MediatorModule());
        serviceCollection.RegisterModule(new OutboxModule(new BiDictionary<string, Type>()));

        serviceCollection.RegisterModule(new QuartzModule());
        serviceCollection.RegisterModule(new SecurityModule(textEncryptionKey));

        serviceCollection.RegisterInstance(executionContextAccessor);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        UserAccessCompositionRoot.SetServiceProvider(_serviceProvider);
    }
}