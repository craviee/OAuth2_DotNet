using BuildingBlocks.Infrastructure.EventBus;
using BuildingBlocks.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Modules.UserAcess.Application.Data;
using Newtonsoft.Json;
using Dapper;

namespace Modules.UserAcess.Infrastructure.Configuration.EventBus;

internal class IntegrationEventGenericHandler<T> : IIntegrationEventHandler<T>
    where T : IntegrationEvent
{
    public async Task Handle(T @event)
    {
        using (var scope = UserAccessCompositionRoot.BeginLifetimeScope())
        {
            using (var connection = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>().GetOpenConnection())
            {
                string type = @event.GetType().FullName;
                var data = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });

                var sql = "INSERT INTO [users].[InboxMessages] (Id, OccurredOn, Type, Data) " +
                          "VALUES (@Id, @OccurredOn, @Type, @Data)";
                
                await connection.ExecuteScalarAsync(sql, new
                {
                    @event.Id,
                    @event.OccurredOn,
                    type,
                    data
                });
            }
        }
    }
}