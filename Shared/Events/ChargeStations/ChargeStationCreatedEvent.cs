namespace Shared.Events.ChargeStations;

public class ChargeStationCreatedEvent
{
    public Guid ChargeStationId { get; set; }
    public string Name { get; set; }
    public int InitialConnectorCount { get; set; }
    public Guid GroupId { get; set; }
}
