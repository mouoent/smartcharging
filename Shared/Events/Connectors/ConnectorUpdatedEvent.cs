namespace Shared.Events.Connectors;

public record ConnectorUpdatedEvent
{
    public Guid NewChargeStationId {  get; set; }
    public Guid OldChargeStationId { get; set; }
    public int OldInternalId { get; set; }
    public int NewInternalId { get; set; }
    public int NewMaxCurrent { get; set; }
    public int OldMaxCurrent { get; set; }
}
