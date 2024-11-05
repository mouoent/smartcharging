using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.Connectors;
using GroupService.Interfaces;

namespace GroupService.EventListeners.Connector;

public class ConnectorDeletedEventListener : BaseEventListener<ConnectorDeletedEvent>
{
    public ConnectorDeletedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection, 
            serviceProvider, 
            queueName: nameof(ConnectorDeletedEvent) + "_GroupService")
    { }

    public override async Task ProcessEventAsync(ConnectorDeletedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var groupRepository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();

            var groups = await groupRepository.GetAllAsync();
            var group = groups.FirstOrDefault(g => g.ChargeStationIds.Contains(eventMessage.ChargeStationId));
            if (group != null)
            {
                // Update the group's current load based on the removed connector's max current
                group.CurrentLoad -= eventMessage.MaxCurrent;
                await groupRepository.UpdateAsync(group);
            }
        }
    }
}
