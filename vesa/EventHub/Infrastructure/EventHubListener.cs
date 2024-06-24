using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Text;

namespace vesa.EventHub.Infrastructure;

public class EventHubListener : EventConsumer, IEventHubListener
{
    protected readonly EventProcessorClient _eventProcessorClient;
    protected readonly IEventProcessor _eventProcessor;
    protected readonly ILogger<EventHubListener> _logger;

    public EventHubListener
    (
        EventProcessorClient eventProcessorClient,
        IEventProcessor eventProcessor,
        ILogger<EventHubListener> logger,
        IEnumerable<IEventMapping> eventMappings
    )
       : base(eventMappings)
    {
        _eventProcessorClient = eventProcessorClient;
        _eventProcessor = eventProcessor;
        _logger = logger;
        _eventProcessorClient.ProcessEventAsync += ProcessEventHandler;
        _eventProcessorClient.ProcessErrorAsync += ProcessErrorHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _eventProcessorClient.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync()
    {
        await _eventProcessorClient.StopProcessingAsync();
    }

    public override async Task<IEnumerable<IEvent>> ConsumeEventsAsync(CancellationToken cancellationToken) => throw new NotSupportedException();

    protected virtual async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        try
        {
            // Write the body of the event to the console window
            var eventJson = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            var consumedEvent = HydrateEvent(eventJson);

            // Process the event
            if (!await _eventProcessor.ProcessAsync(consumedEvent, eventArgs.CancellationToken))
            {
                //TODO: Throw to dead letter queue. Processor does not recognize the event
                //...
            }

            // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
            // TODO: if this becomes a bottleneck, we can implement
            // (1) batch checkpointing by event proccessed count or time duration
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }
        catch (Exception ex)
        {
            var properties = String.Join(", ", eventArgs.Data.Properties.Select(x => $"[{x.Key}]:[{x.Value}]"));
            _logger.LogError
            (
                ex,
                "Failed during event processing.  Will not be retried.  Message Id {messageId} / partitionKey {partitionKey}. Properties {properties}",
                eventArgs.Data.MessageId,
                eventArgs.Data.PartitionKey,
                properties
            );
        }
    }

    protected Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        _logger.LogError
        (
            eventArgs.Exception,
            "Error processing event in partition [ {eventPartitionId} ]. Operation: [ {eventOperation} ].",
            eventArgs.PartitionId,
            eventArgs.Operation ?? "Unknown"
        );

        return Task.CompletedTask;
    }

}
