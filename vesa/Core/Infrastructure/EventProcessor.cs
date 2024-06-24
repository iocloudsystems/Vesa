using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class EventProcessor : IEventProcessor
{
    private readonly IDictionary<string, IEventObservers> _eventObserversDictionary = new Dictionary<string, IEventObservers>();

    public EventProcessor(IEnumerable<IEventObservers> eventObserversCollection)
    {
        foreach (var eventObservers in eventObserversCollection)
        {
            var key = eventObservers.GetType().FullName;
            if (!_eventObserversDictionary.ContainsKey(key))
            {
                _eventObserversDictionary.Add(key, eventObservers);
            }
        }
    }

    public async Task<bool> ProcessAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var processed = false;
        Type eventObserversType = typeof(EventHandlerObservers<>);
        Type genericEventObserversType = eventObserversType.MakeGenericType(@event.GetType());
        if (_eventObserversDictionary.ContainsKey(genericEventObserversType.FullName))
        {
            var eventObservers = _eventObserversDictionary[genericEventObserversType.FullName];
            await eventObservers.NotifyAsync(@event, cancellationToken);
            processed = true;
        }
        return processed;
    }
}



