namespace Shared.Events.ChargeStations;

public record ChargeStationDeletedEvent
{
    public Guid ChargeStationId { get; set; }
}
