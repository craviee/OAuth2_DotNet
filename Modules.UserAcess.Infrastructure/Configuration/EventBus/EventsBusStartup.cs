using BuildingBlocks.Infrastructure.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Modules.UserAcess.Infrastructure.Configuration.EventBus;

public static class EventsBusStartup
{
    public static void Initialize(
        ILogger logger)
    {
        SubscribeToIntegrationEvents(logger);
    }

    private static void SubscribeToIntegrationEvents(ILogger logger)
    {
        var eventBus = UserAccessCompositionRoot.BeginLifetimeScope().ServiceProvider.GetRequiredService<IEventsBus>();

        // SubscribeToIntegrationEvent<MemberCreatedIntegrationEvent>(eventBus, logger);
    }

    private static void SubscribeToIntegrationEvent<T>(IEventsBus eventBus, ILogger logger)
        where T : IntegrationEvent
    {
        logger.Information("Subscribe to {@IntegrationEvent}", typeof(T).FullName);
        eventBus.Subscribe(
            new IntegrationEventGenericHandler<T>());
    }
}