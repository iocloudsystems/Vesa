using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Exceptions;
using vesa.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace vesa.Core.Infrastructure;

public abstract class EventStore : IEventStore
{
    protected readonly ILogger<EventStore> _logger;

    protected EventStore(ILogger<EventStore> logger)
    {
        _logger = logger;
    }

    public abstract Task<IEnumerable<IEvent>> GetEventsAsync(string subject, CancellationToken cancellationToken = default);

    public abstract Task<int> CountEventsAsync(string subject, CancellationToken cancellationToken = default);

    public abstract Task<IEvent> GetEventAsync(string eventId, string subject, CancellationToken cancellationToken = default);

    public async Task<bool> EventExistsAsync(string eventId, string subject, CancellationToken cancellationToken = default)
        => await GetEventAsync(eventId, subject, cancellationToken) != null;

    public abstract Task<bool> IdempotencyCheckAsync(string subject, string eventTypeName, string idempotencyToken, CancellationToken cancellationToken = default);

    public virtual async Task AddEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        if (!(@event is ExceptionEvent) && !(@event.EventTypeName.Contains("BuiltEvent")) && string.IsNullOrWhiteSpace(@event.Subject))
        {
            throw new NoEventSubjectException(@event.Id);
        }
        if (await IdempotencyCheckAsync(@event.Subject, @event.EventTypeName, @event.IdempotencyToken, cancellationToken))
        {
            var sequencedEvent = await SetEventSequenceNumber(@event, cancellationToken);
            await StoreEventAsync(sequencedEvent, cancellationToken);
        }
    }

    protected virtual async Task<IEvent> SetEventSequenceNumber(IEvent @event, CancellationToken cancellationToken)
    {
        // Set the new sequence number
        var sequenceNumber = await GetNextSequenceNumberAsync(@event.Subject, cancellationToken);
        return @event.SetNewInstanceProperty(JsonPropertyName.SequenceNumber, sequenceNumber);
    }

    protected async Task<int> GetNextSequenceNumberAsync(string subject, CancellationToken cancellationToken)
    => (await CountEventsAsync(subject, cancellationToken)) + 1;

    public abstract Task StoreEventAsync(IEvent @event, CancellationToken cancellationToken = default);

    public virtual async Task AddEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        var preparedEvents = new List<IEvent>();
        foreach (var @event in events)
        {
            if (!(@event is ExceptionEvent) && !(@event.EventTypeName.Contains("BuiltEvent")) && string.IsNullOrWhiteSpace(@event.Subject))
            {
                throw new NoEventSubjectException(@event.Id);
            }
            if (await IdempotencyCheckAsync(@event.Subject, @event.EventTypeName, @event.IdempotencyToken, cancellationToken))
            {
                preparedEvents.Add(await SetEventSequenceNumber(@event, cancellationToken));
            }
        }
        await StoreEventsAsync(preparedEvents, cancellationToken);
    }

    public abstract Task StoreEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<string>> GetSubjectsAsync(CancellationToken cancellationToken = default);

}