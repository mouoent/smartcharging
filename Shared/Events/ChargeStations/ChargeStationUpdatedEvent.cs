namespace Shared.Events.ChargeStations;

public record ChargeStationUpdatedEvent
{
    public Guid ChargeStationId { get; init; }
    public Guid OldGroupId { get; init; }
    public Guid NewGroupId { get; init; }
    public int ChargeStationTotalCapacity { get; init; } 
}
