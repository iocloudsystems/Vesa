using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class EventHandlerObservers<TEvent> : IEventObservers
    where TEvent : IEvent
{
    private readonly IEnumerable<IEventHandler<TEvent>> _eventHandlers;

    public EventHandlerObservers(IEnumerable<IEventHandler<TEvent>> eventHandlers)
    {
        _eventHandlers = eventHandlers;
    }

    public async Task NotifyAsync(IEvent @event, CancellationToken cancellationToken)
    {
        if (@event is TEvent observedEvent)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                await eventHandler.HandleAsync(observedEvent, cancellationToken);
            }
        }
    }
}