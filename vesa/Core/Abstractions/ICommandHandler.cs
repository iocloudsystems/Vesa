namespace vesa.Core.Abstractions;

public interface ICommandHandler<TCommand>
    where TCommand : ICommand
{
    Task<IEnumerable<IEvent>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<TCommand, TStateView> : ICommandHandler<TCommand>
    where TCommand : ICommand
    where TStateView : class, IStateView, new()
{
}
