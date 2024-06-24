namespace vesa.Core.Abstractions;

public interface IEventObservers
{
    Task NotifyAsync(IEvent @event, CancellationToken cancellationToken);
}