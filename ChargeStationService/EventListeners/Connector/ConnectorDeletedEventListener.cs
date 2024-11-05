using RabbitMQ.Client;
using Infrastructure.EventListeners;
using Shared.Events.Connectors;
using ChargeStationService.Interfaces;

namespace ChargeStationService.EventListeners.ChargeStation;

public class ConnectorDeletedEventListener : BaseEventListener<ConnectorDeletedEvent>
{
    public ConnectorDeletedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection,
            serviceProvider,
            queueName: nameof(ConnectorDeletedEvent) + "_ChargeStationService")
    { }

    public override async Task ProcessEventAsync(ConnectorDeletedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var chargeStationRepository = scope.ServiceProvider.GetRequiredService<IChargeStationRepository>();

            var chargeStation = await chargeStationRepository.GetByIdAsync(eventMessage.ChargeStationId);
            if (chargeStation != null)
            {
                // Remove the connector's internal ID from the list
                chargeStation.InternalConnectorIds.Remove(eventMessage.InternalConnectorId);

                // Update the charge station in the database
                await chargeStationRepository.UpdateAsync(chargeStation);                
            }
        }
    }
}
