using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Helpers;
using vesa.Cosmos.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Cosmos.Infrastructure;

public class EventStoreChangeFeedProcessorFactory : IChangeFeedProcessorFactory<JObject>
{
    private readonly IChangeFeedProcessorConfiguration _configuration;
    private readonly CosmosClient _client;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<EventStoreChangeFeedProcessorFactory> _logger;

    public EventStoreChangeFeedProcessorFactory
    (
        IChangeFeedProcessorConfiguration configuration,
        CosmosClient client,
        IEventProcessor eventProcessor,
        ILogger<EventStoreChangeFeedProcessorFactory> logger
    )
    {
        _configuration = configuration;
        _client = client;
        _eventProcessor = eventProcessor;
        _logger = logger;

        Builder = GetSourceContainer()
            .GetChangeFeedProcessorBuilder<JObject>(processorName: _configuration.ProcessorName, onChangesDelegate: HandleChangesAsync)
            .WithInstanceName(Environment.MachineName)
            .WithLeaseContainer(GetLeaseContainer())
            .WithLeaseAcquireNotification(onLeaseAcquiredAsync)
            .WithLeaseReleaseNotification(onLeaseReleaseAsync)
            .WithErrorNotification(onErrorAsync);

        if (configuration.StartDateTimeOffset != null)
        {
            Builder = Builder.WithStartTime(DateTime.SpecifyKind(Convert.ToDateTime(configuration.StartDateTimeOffset?.ToString()), DateTimeKind.Utc));
        }
    }

    public ChangeFeedProcessorBuilder Builder { get; init; }

    public ChangeFeedProcessor CreateProcessor() => Builder.Build();
    protected virtual async Task HandleChangesAsync
    (
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<JObject> changes,
        CancellationToken cancellationToken
    )
    {
        foreach (var item in changes)
        {
            var eventTypeName = (string)item[JsonPropertyName.EventTypeName];
            var storedEvent = (IEvent)JsonConvert.DeserializeObject(item.ToString(), TypeHelper.GetType(eventTypeName));

            //TODO: if there are prior failures and we restart, we do NOT want to re-notify observers who have been notified
            await _eventProcessor.ProcessAsync(storedEvent, cancellationToken);
        }
    }

    protected Container.ChangeFeedMonitorLeaseAcquireDelegate onLeaseAcquiredAsync = (string leaseToken) =>
    {
        Console.WriteLine($"Lease {leaseToken} is acquired and will start processing");
        return Task.CompletedTask;
    };

    protected Container.ChangeFeedMonitorLeaseReleaseDelegate onLeaseReleaseAsync = (string leaseToken) =>
    {
        Console.WriteLine($"Lease {leaseToken} is released and processing is stopped");
        return Task.CompletedTask;
    };

    protected Container.ChangeFeedMonitorErrorDelegate onErrorAsync = (string leaseToken, Exception exception) =>
    {
        if (exception is ChangeFeedProcessorUserException userException)
        {
            Console.WriteLine($"Lease {leaseToken} processing failed with unhandled exception from user delegate {userException.InnerException}");
        }
        else
        {
            Console.WriteLine($"Lease {leaseToken} failed with {exception}");
        }
        return Task.CompletedTask;
    };

    protected Container GetSourceContainer()
    {
        var database = _client.GetDatabase(_configuration.SourceDatabaseName);
        var container = database.GetContainer(_configuration.SourceContainerName);
        return container;
    }

    protected Container GetLeaseContainer()
    {
        var database = _client.GetDatabase(_configuration.LeaseDatabaseName);
        var container = database.GetContainer(_configuration.LeaseContainerName);
        return container;
    }
}

