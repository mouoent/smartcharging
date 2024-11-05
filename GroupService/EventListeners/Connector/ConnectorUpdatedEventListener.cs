using GroupService.Interfaces;
using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.Connectors;

namespace GroupService.EventListeners.Connector;

public class ConnectorUpdatedEventListener : BaseEventListener<ConnectorUpdatedEvent>
{
    public ConnectorUpdatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection, 
            serviceProvider, 
            queueName: nameof(ConnectorUpdatedEvent) + "_GroupService")
    { }

    public override async Task ProcessEventAsync(ConnectorUpdatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();

            var groups = await repository.GetAllAsync();

            
            var oldGroup = groups.FirstOrDefault(g => g.ChargeStationIds.Contains(eventMessage.OldChargeStationId));
            var newGroup = groups.FirstOrDefault(g => g.ChargeStationIds.Contains(eventMessage.NewChargeStationId));

            // Handle the old group, if the connector is moved to a new charge station
            if (oldGroup.Id != newGroup.Id)
            {
                oldGroup.CurrentLoad -= eventMessage.OldMaxCurrent;
                await repository.UpdateAsync(oldGroup);

                newGroup.CurrentLoad += eventMessage.NewMaxCurrent;
                await repository.UpdateAsync(newGroup);
            }           

            // If the connector is only being updated within the same charge station
            if (oldGroup.Id == newGroup.Id)
            {
                oldGroup.CurrentLoad -= eventMessage.OldMaxCurrent;
                oldGroup.CurrentLoad += eventMessage.NewMaxCurrent;
                await repository.UpdateAsync(oldGroup);
            }
        }
    }
}
