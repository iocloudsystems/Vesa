namespace vesa.Core.Abstractions;

public interface IEventProcessor
{
    Task<bool> ProcessAsync(IEvent @event, CancellationToken cancellationToken = default);
}
