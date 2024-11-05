using GroupService.Interfaces;
using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.ChargeStations;

namespace GroupService.EventListeners.ChargeStation;

public class ChargeStationCreatedEventListener : BaseEventListener<ChargeStationCreatedEvent>
{
    public ChargeStationCreatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
       : base(configuration,
           connection,
           serviceProvider, 
           queueName: nameof(ChargeStationCreatedEvent) + "_GroupService")
    { }

    public override async Task ProcessEventAsync(ChargeStationCreatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var groupRepository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();

            var group = await groupRepository.GetByIdAsync(eventMessage.GroupId);
            if (group != null)
            {
                // Add the charge station's ID to the list
                group.ChargeStationIds.Add(eventMessage.ChargeStationId);

                // Update the group in the database
                await groupRepository.UpdateAsync(group);
            }
        }
    }
}

