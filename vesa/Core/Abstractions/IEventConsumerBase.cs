namespace vesa.Core.Abstractions;

public interface IEventConsumerBase
{
    Task<IEnumerable<IEvent>> ConsumeEventsAsync(CancellationToken cancellationToken);
}
