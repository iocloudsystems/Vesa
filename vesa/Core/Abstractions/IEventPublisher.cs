namespace vesa.Core.Abstractions;

public interface IEventPublisher
{
    Task PublishEventAsync(IEvent @event, CancellationToken cancellation = default);
    Task PublishCloudEventAsync<TCloudEventData>(TCloudEventData eventData, string source, string type = null)
        where TCloudEventData : IEvent;
}
