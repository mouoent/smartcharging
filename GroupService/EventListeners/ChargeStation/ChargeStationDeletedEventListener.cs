using GroupService.Interfaces;
using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.ChargeStations;

namespace GroupService.EventListeners.ChargeStation;

public class ChargeStationDeletedEventListener : BaseEventListener<ChargeStationDeletedEvent>
{
    public ChargeStationDeletedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
   : base(configuration,
        connection,
        serviceProvider, 
        queueName: nameof(ChargeStationDeletedEvent) + "_GroupService")
    { }

    public override async Task ProcessEventAsync(ChargeStationDeletedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var groupRepository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();

            var groups = await groupRepository.GetAllAsync();
            var group = groups.FirstOrDefault(g => g.ChargeStationIds.Contains(eventMessage.ChargeStationId));
            if (group != null)
            {
                // Remove the charge station's ID from the list
                group.ChargeStationIds.Remove(eventMessage.ChargeStationId);

                // Update the group in the database
                await groupRepository.UpdateAsync(group);
            }
        }
    }
}
