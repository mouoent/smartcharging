using ChargeStationService.Interfaces;
using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.Connectors;

namespace ChargeStationService.EventListeners;

public class ConnectorCreatedEventListener : BaseEventListener<ConnectorCreatedEvent>
{
    public ConnectorCreatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection,
            serviceProvider, 
            queueName: nameof(ConnectorCreatedEvent) + "_ChargeStationService")
    { }

    public override async Task ProcessEventAsync(ConnectorCreatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IChargeStationRepository>();

            // Retrieve the ChargeStation associated with the connector
            var chargeStation = await repository.GetByIdAsync(eventMessage.GroupId);
            if (chargeStation != null)
            {
                // Add the new connector's internal ID and update the load if needed
                chargeStation.InternalConnectorIds.Add(eventMessage.InternalId);
                
                await repository.UpdateAsync(chargeStation);
            }
        }
    }
}
