using ChargeStationService.Interfaces;
using RabbitMQ.Client;
using Infrastructure.EventListeners;
using Shared.Events.ChargeStations;
using Shared.Events.Groups;
using Shared.Interfaces;

namespace ChargeStationService.EventListeners.Group;

public class GroupDeletedEventListener : BaseEventListener<GroupDeletedEvent>
{
    public GroupDeletedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider)
        : base(configuration,
            connection,
            serviceProvider, 
            queueName: nameof(GroupDeletedEvent) + "_ChargeStation")
    { }

    public override async Task ProcessEventAsync(GroupDeletedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IChargeStationRepository>();

            // Delete all ChargeStations associated with the deleted GroupId
            await repository.DeleteByGroupIdAsync(eventMessage.GroupId);

            // Publish ChargeStationDeletedEvent for each to be deleted ChargeStation
            var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
            var chargeStations = await repository.GetByGroupIdAsync(eventMessage.GroupId);
            foreach (var chargeStation in chargeStations)
            {
                var chargeStationDeletedEvent = new ChargeStationDeletedEvent { ChargeStationId = chargeStation.Id };
                await eventPublisher.PublishAsync(chargeStationDeletedEvent);
            }            
        }
    }
}
