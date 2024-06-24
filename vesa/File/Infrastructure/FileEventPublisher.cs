using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using Newtonsoft.Json;

namespace vesa.File.Infrastructure;

public class FileEventPublisher : FileEventPublisherBase, IEventPublisher
{
    public FileEventPublisher(string path) : base(path)
    {
        Path = path;
    }

    public async Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var eventJson = JsonConvert.SerializeObject(@event);
        await PublishCoreAsync(@event, eventJson, cancellationToken);
    }

    public async Task PublishCloudEventAsync<TEventData>(TEventData eventData, string source, string type = null)
         where TEventData : IEvent
    {
        var cloudEvent = new CloudEvent<TEventData>
        (
            eventData.Id.ToString(),
            source,
            type ?? eventData.GetType().Name,
            eventData.Subject,
            eventData.EventDate,
            eventData
        );
        var eventJson = JsonConvert.SerializeObject(cloudEvent);
        await PublishCoreAsync(eventData, eventJson);
    }
}
