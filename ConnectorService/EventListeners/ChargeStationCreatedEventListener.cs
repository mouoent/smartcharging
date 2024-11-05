using ConnectorService.Interfaces;
using ConnectorService.Models.Connector;
using RabbitMQ.Client;
using Infrastructure.EventListeners;
using Shared.Events.ChargeStations;

namespace ConnectorService.EventListeners;

public class ChargeStationCreatedEventListener : BaseEventListener<ChargeStationCreatedEvent>
{
    public ChargeStationCreatedEventListener(IConfiguration configuration, IConnection connection, IServiceProvider serviceProvider) 
        : base(configuration,
            connection, 
            serviceProvider, 
            queueName: nameof(ChargeStationCreatedEvent) + "_ConnectorService")
    {}

    public override async Task ProcessEventAsync(ChargeStationCreatedEvent eventMessage)
    {
        using (var scope = _serviceProvider.CreateScope()) 
        {
            var repository = scope.ServiceProvider.GetRequiredService<IConnectorRepository>();

            // Create the initial connectors for the new charge station
            for (int i = 1; i <= eventMessage.InitialConnectorCount; i++)
            {
                var connector = new Connector
                {
                    ChargeStationId = eventMessage.ChargeStationId,
                    InternalId = i, // Assign a unique internal ID per charge station
                    MaxCurrent = 0 
                };

                await repository.AddAsync(connector);
            }
        }        
    }
}
