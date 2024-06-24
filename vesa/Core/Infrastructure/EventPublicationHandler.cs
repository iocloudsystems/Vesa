using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class EventPublicationHandler<TEvent> : IEventHandler<TEvent>
    where TEvent : class, IEvent
{
    private readonly IEventPublisher _eventPublisher;

    public EventPublicationHandler
    (
        IEventPublisher eventPublisher
    )
    {
        _eventPublisher = eventPublisher;
    }

    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        await _eventPublisher.PublishEventAsync(@event, cancellationToken);
    }
}
