namespace Shared.Events.Connectors;

public class ConnectorDeletedEvent
{
    public Guid ChargeStationId { get; set; }
    public int InternalConnectorId { get; set; }
    public int MaxCurrent { get; set; }
}
