using vesa.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace vesa.Core.Infrastructure;

public abstract class StateViewStore<TStateView> : IStateViewStore<TStateView>
    where TStateView : class, IStateView, new()
{
    protected readonly IEventStore _eventStore;
    protected readonly ILogger<StateViewStore<TStateView>> _logger;

    public StateViewStore(IEventStore eventStore, ILogger<StateViewStore<TStateView>> logger)
    {
        _eventStore = eventStore;
        _logger = logger;
    }

    public virtual async Task<TStateView> HydrateStateViewAsync(string subject, DateTimeOffset? upToDate = null, CancellationToken cancellationToken = default)
    {
        IStateView currentState = new TStateView();
        if (!subject.StartsWith(currentState.Subject))
        {
            throw new ArgumentException($"Subject {subject} does not match the pattern of {typeof(TStateView).Name}.Subject {currentState.Subject}*");
        }
        var events = await _eventStore.GetEventsAsync(subject, cancellationToken);
        if (upToDate != null)
        {
            events = events.Where(e => e.EventDate <= upToDate);
        }
        foreach (var @event in events)
        {
            if (string.IsNullOrWhiteSpace(currentState.Subject))
            {
                throw new Exception("StateView Subject cannot be null or empty");
            }
            if (string.IsNullOrWhiteSpace(@event.Subject))
            {
                throw new Exception("Event Subject cannot be null or empty");
            }
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            var stateView = currentState.ApplyEvent(@event);
            if (stateView != null)
            {
                currentState = stateView;
            }
        }
        return (TStateView)currentState;
    }

    public abstract Task<TStateView> GetStateViewAsync(string subject, CancellationToken cancellationToken = default);

    public abstract Task SaveStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default);

    public abstract Task DeleteStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default);

    public abstract Task DeleteStateViewAsync(string subject, CancellationToken cancellationToken);

    public virtual async Task<TStateView> UpdateStateViewAsync(string subject, IEvent @event, CancellationToken cancellationToken = default)
    {
        TStateView currentState = default;
        if (@event.Subject == subject)
        {
            currentState = await GetStateViewAsync(subject, cancellationToken);
            if (currentState == null)
            {
                currentState = new TStateView();
            }
            currentState = (TStateView)currentState.ApplyEvent(@event);
            await SaveStateViewAsync(currentState, cancellationToken);
        }
        return currentState;
    }
}