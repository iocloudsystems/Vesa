using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class EventPropagationHandler<TEvent, TDefaultStateView> : IEventHandler<TEvent>
    where TEvent : class, IEvent
    where TDefaultStateView : class, IStateView, new()
{
    private readonly IFactory<TDefaultStateView> _defaultStateViewFactory;
    private readonly IEventPropagationService<TEvent> _eventPropagationService;
    protected readonly IEventStore _eventStore;

    public EventPropagationHandler
    (
        IFactory<TDefaultStateView> defaultStateViewFactory,
        IEventPropagationService<TEvent> eventPropagationService,
        IEventStore eventStore
    )
    {
        _defaultStateViewFactory = defaultStateViewFactory;
        _eventPropagationService = eventPropagationService;
        _eventStore = eventStore;
    }

    public virtual async Task HandleAsync(TEvent @event, CancellationToken cancellationToken)
    {
        // if the event has the default subject
        var defaultStateView = _defaultStateViewFactory.Create();

        // this check prevents duplication of event creation for non-default subjects
        if (@event.SubjectPrefix == defaultStateView.SubjectPrefix)
        {
            // save the same event with different subjects to feed multiple state views
            await AddEventsWithStateViewSubjectsAsync(@event, cancellationToken);
        }
    }

    protected async Task AddEventsWithStateViewSubjectsAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        // save the same event with different subjects to feed multiple state views
        var propagationEvents = _eventPropagationService.GetPropagationEvents(@event);
        foreach (var propagationEvent in propagationEvents)
        {
            if (!(await _eventStore.EventExistsAsync(propagationEvent.Id, propagationEvent.Subject, cancellationToken)))
            {
                await _eventStore.AddEventAsync(propagationEvent, cancellationToken);
            }
        }
    }
}
