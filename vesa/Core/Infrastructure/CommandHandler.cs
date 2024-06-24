using vesa.Core.Abstractions;
using vesa.Core.Exceptions;

namespace vesa.Core.Infrastructure;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    private readonly IServiceProvider _serviceProvider;
    protected readonly IDomain<TCommand> _domain;
    protected readonly IEventStore _eventStore;

    public CommandHandler
    (
        IServiceProvider serviceProvider,
        IDomain<TCommand> domain,
        IEventStore eventStore
    )
    {
        _serviceProvider = serviceProvider;
        _domain = domain;
        _eventStore = eventStore;
    }

    public virtual async Task<IEnumerable<IEvent>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var events = await _domain.ProcessAsync(command);
        await _eventStore.AddEventsAsync(events, cancellationToken);
        var exceptionEvent = events.FirstOrDefault(e => e is ExceptionEvent) as ExceptionEvent;
        if (exceptionEvent != null)
        {
            throw exceptionEvent.Exception;
        }
        return events;
    }
}

public abstract class CommandHandler<TCommand, TStateView> : ICommandHandler<TCommand, TStateView>
    where TCommand : ICommand
    where TStateView : class, IStateView, new()
{
    protected readonly IStateViewStore<TStateView> _stateViewStore;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly IDomain<TCommand, TStateView> _domain;
    protected readonly IEventStore _eventStore;

    public CommandHandler
    (
        IStateViewStore<TStateView> stateViewStore,
        IServiceProvider serviceProvider,
        IDomain<TCommand, TStateView> domain,
        IEventStore eventStore
    )
    {
        _stateViewStore = stateViewStore;
        _serviceProvider = serviceProvider;
        _domain = domain;
        _eventStore = eventStore;
    }

    public virtual async Task<IEnumerable<IEvent>> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var subject = GetStateViewSubject(command);
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new NoSubjectException();
        }
        var stateView = await _stateViewStore.HydrateStateViewAsync(subject);
        CheckConcurrency(command, stateView);
        var events = await _domain.ProcessAsync(command, stateView);
        await _eventStore.AddEventsAsync(events, cancellationToken);
        var exceptionEvent = events.FirstOrDefault(e => e is ExceptionEvent) as ExceptionEvent;
        if (exceptionEvent != null)
        {
            throw exceptionEvent.Exception;
        }
        return events;
    }

    protected abstract string GetStateViewSubject(TCommand command);

    protected void CheckConcurrency(TCommand command, TStateView stateView)
    {
        if (command.LastEventSequenceNumber != stateView.LastEventSequenceNumber)
        {
            throw new StaleStateViewException(command.Id);
        }
    }
}

