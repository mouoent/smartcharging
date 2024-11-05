using GroupService.Interfaces;
using Infrastructure.EventListeners;
using RabbitMQ.Client;
using Shared.Events.ChargeStations;

namespace GroupService.EventListeners.ChargeStation;

public class ChargeStationUpdatedEventListener : BaseEventListener<ChargeStationUpdatedEvent>
{
    public ChargeStationUpdatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection,
            serviceProvider, 
            queueName: nameof(ChargeStationUpdatedEvent) + "_GroupService")
    { }

    public override async Task ProcessEventAsync(ChargeStationUpdatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IGroupRepository>();

            var oldGroup = await repository.GetByIdAsync(eventMessage.OldGroupId);
            var newGroup = await repository.GetByIdAsync(eventMessage.NewGroupId);

            // Update the old group if it has been changed
            if (oldGroup != null && eventMessage.NewGroupId != eventMessage.OldGroupId)
            {
                oldGroup.ChargeStationIds.Remove(eventMessage.ChargeStationId);
                oldGroup.CurrentLoad -= eventMessage.ChargeStationTotalCapacity;
                await repository.UpdateAsync(oldGroup);

                // Update the new group if it's different from the old group
                if (eventMessage.NewGroupId != Guid.Empty && newGroup != null)
                {                
                    newGroup.ChargeStationIds.Add(eventMessage.ChargeStationId);
                    newGroup.CurrentLoad += eventMessage.ChargeStationTotalCapacity;
                    await repository.UpdateAsync(newGroup);
                }
                
            }          
        }
    }
}
