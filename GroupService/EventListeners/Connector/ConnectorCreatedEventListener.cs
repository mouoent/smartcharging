using GroupService.Interfaces;
using RabbitMQ.Client;
using Infrastructure.EventListeners;
using Shared.Events.Connectors;

namespace GroupService.EventListeners.Connector;

public class ConnectorCreatedEventListener : BaseEventListener<ConnectorCreatedEvent>
{
    public ConnectorCreatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection,
            serviceProvider, 
            queueName: nameof(ConnectorCreatedEvent) + "_GroupService")
    { }

    public override async Task ProcessEventAsync(ConnectorCreatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();

            // Update the group's current load based on the added connector's max current
            var group = await repository.GetByIdAsync(eventMessage.GroupId);
            if (group != null)
            {
                group.CurrentLoad += eventMessage.MaxCurrent;
                await repository.UpdateAsync(group);
            }
        }
    }
}
