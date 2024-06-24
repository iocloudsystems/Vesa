namespace vesa.Core.Abstractions;

public interface IDomain<TCommand>
    where TCommand : ICommand
{
    Task<IEnumerable<IEvent>> ProcessAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IDomain<TCommand, TStateView>
    where TCommand : ICommand
    where TStateView : IStateView
{
    Task<IEnumerable<IEvent>> ProcessAsync(TCommand command, TStateView stateView, CancellationToken cancellation = default);
}
