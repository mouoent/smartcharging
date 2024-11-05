using ConnectorService.Interfaces;
using RabbitMQ.Client;
using Infrastructure.EventListeners;
using Shared.Events.ChargeStations;

namespace ConnectorService.EventListeners;

public class ChargeStationDeletedEventListener : BaseEventListener<ChargeStationDeletedEvent>
{
    public ChargeStationDeletedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection, 
            serviceProvider, 
            queueName: nameof(ChargeStationDeletedEvent) + "_ConnectorService")
    {}

    public override async Task ProcessEventAsync(ChargeStationDeletedEvent chargeStationDeletedEvent)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IConnectorRepository>();

            // Find all Connectors associated with the deleted ChargeStation and delete them
            await repository.DeleteByChargeStationIdAsync(chargeStationDeletedEvent.ChargeStationId);
        }            
    }    
}
