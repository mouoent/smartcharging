using ChargeStationService.Interfaces;
using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.Connectors;

namespace ChargeStationService.EventListeners;

public class ConnectorUpdatedEventListener : BaseEventListener<ConnectorUpdatedEvent>
{
    public ConnectorUpdatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration, 
            connection,
            serviceProvider, 
            queueName: nameof(ConnectorUpdatedEvent) + "_ChargeStationService")
    { }

    public override async Task ProcessEventAsync(ConnectorUpdatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IChargeStationRepository>();

            
            
            if (eventMessage.OldChargeStationId != eventMessage.NewChargeStationId)
            {
                // Retrieve the old ChargeStation and update its state
                var oldChargeStation = await repository.GetByIdAsync(eventMessage.OldChargeStationId);
                if (oldChargeStation != null)
                {
                    oldChargeStation.InternalConnectorIds.Remove(eventMessage.OldInternalId);
                    await repository.UpdateAsync(oldChargeStation);
                }

                // Retrieve the new ChargeStation and update its state
                var newChargeStation = await repository.GetByIdAsync(eventMessage.NewChargeStationId);
                if (newChargeStation != null)
                {
                    newChargeStation.InternalConnectorIds.Add(eventMessage.NewInternalId);
                    await repository.UpdateAsync(newChargeStation);
                }
            }            
        }
    }
}
