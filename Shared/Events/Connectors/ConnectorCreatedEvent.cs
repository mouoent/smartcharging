namespace Shared.Events.Connectors;

public record ConnectorCreatedEvent
{
    public Guid GroupId;
    public int InternalId;
    public int MaxCurrent;
}
