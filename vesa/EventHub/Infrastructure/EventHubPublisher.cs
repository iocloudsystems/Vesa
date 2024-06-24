using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace vesa.EventHub.Infrastructure;
public class EventHubPublisher : IEventPublisher
{
    protected readonly EventHubProducerClient _client;
    protected readonly ILogger<EventHubPublisher> _logger;

    public EventHubPublisher(EventHubProducerClient client, ILogger<EventHubPublisher> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            EventDataBatch eventBatch = await _client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = @event.Subject });
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event))));
            await _client.SendAsync(eventBatch, cancellationToken);
        }

        catch (EventHubsException ex)
        {
            _logger.LogError($"{nameof(EventHubsException)} - error details: {ex.Message}");
            throw;
        }
    }

    public async Task PublishCloudEventAsync<TCloudEventData>(TCloudEventData @event, string source, string type = null)
        where TCloudEventData : IEvent
    {
        try
        {
            EventDataBatch eventBatch = await _client.CreateBatchAsync(new CreateBatchOptions { PartitionKey = @event.Subject });
            var cloudEvent = new CloudEvent<TCloudEventData>(source, type ?? @event.EventTypeName, @event.Subject, @event.EventDate, @event);
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cloudEvent))));
            await _client.SendAsync(eventBatch);
        }
        catch (EventHubsException ex)
        {
            _logger.LogError($"{nameof(EventHubsException)} - error details: {ex.Message}");
            throw;
        }
    }
}