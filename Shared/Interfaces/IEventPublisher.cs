namespace Shared.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent eventMessage);
}
