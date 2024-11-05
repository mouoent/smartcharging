namespace Shared.Events.Groups;

public record GroupDeletedEvent
{
    public Guid GroupId { get; set; }
}
