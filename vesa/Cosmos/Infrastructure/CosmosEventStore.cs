using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Exceptions;
using vesa.Core.Helpers;
using vesa.Core.Infrastructure;
using vesa.Cosmos.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Cosmos.Infrastructure;

public class CosmosEventStore : EventStore, IEventStore, IDisposable
{
    private readonly CosmosClient _client;
    private readonly ICosmosContainerConfiguration _configuration;

    public CosmosEventStore
    (
        CosmosClient client,
        ICosmosContainerConfiguration configuration,
        ILogger<CosmosEventStore> logger
    )
        : base(logger)
    {
        _client = client;
        _configuration = configuration;
    }

    public override async Task<IEnumerable<IEvent>> GetEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        var container = GetContainer();
        using (var feedIterator = container.GetItemLinqQueryable<JObject>().Where(e => (string)e[JsonPropertyName.Subject] == subject).ToFeedIterator())
        {
            var items = new List<JObject>();
            while (feedIterator.HasMoreResults)
            {
                items.AddRange(await feedIterator.ReadNextAsync(cancellationToken));
            }
            var events = items
                        .Select(e => (IEvent)JsonConvert.DeserializeObject(e.ToString(), TypeHelper.GetType((string)e[JsonPropertyName.EventTypeName])))
                        .OrderBy(e => e.SequenceNumber);
            return events;
        }
    }

    public override async Task<int> CountEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        var container = GetContainer();
        var query = container.GetItemQueryIterator<int>(new QueryDefinition($"SELECT VALUE count(1) FROM c WHERE c.partitionKey = '{subject}'"));
        FeedResponse<int> response = default;
        try
        {
            response = await query.ReadNextAsync();
        }
        catch (Exception ex)
        {
            throw;
        }
        return response.Resource.FirstOrDefault(0);
    }

    public override async Task<IEvent> GetEventAsync(string eventId, string subject, CancellationToken cancellationToken = default)
    {
        IEvent @event = default;
        var container = GetContainer();
        using (var feedIterator = container.GetItemLinqQueryable<JObject>()
                .Where(e => (string)e[JsonPropertyName.Subject] == subject && (string)e[JsonPropertyName.Id] == eventId)
                .ToFeedIterator())
        {
            while (feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync())
                {
                    @event = (IEvent)JsonConvert.DeserializeObject(item.ToString(), TypeHelper.GetType((string)item[JsonPropertyName.EventTypeName]));
                    break;
                }
            }
            return @event;
        }
    }

    public override async Task<bool> IdempotencyCheckAsync(string subject, string eventTypeName, string idempotencyToken, CancellationToken cancellationToken = default)
    {
        var check = true;
        if (!string.IsNullOrWhiteSpace(idempotencyToken))
        {
            var container = GetContainer();
            using (var feedIterator = container.GetItemLinqQueryable<JObject>()
                    .Where(e => (string)e[JsonPropertyName.Subject] == subject &&
                                (string)e[JsonPropertyName.EventTypeName] == eventTypeName &&
                                (string)e[JsonPropertyName.IdempotencyToken] == idempotencyToken)
                    .ToFeedIterator())
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        check = false;
                        break;
                    }
                }
            }
        }
        return check;
    }

    public override async Task StoreEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(@event.Subject))
        {
            throw new NoEventSubjectException(@event.Id);
        }

        try
        {
            var container = GetContainer();
            var partitionKey = new PartitionKey(@event.Subject);
            var jobject = (JObject)JToken.FromObject(@event);
            var itemResponse = await container.CreateItemAsync(jobject, partitionKey, null, cancellationToken);
        }
        catch (CosmosException ex)
        {
            _logger.LogError($"New event with ID: {@event.Id} was not added successfully - error details: {ex.Message}");
            throw;
        }
    }

    public override async Task StoreEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        if (events.Any())
        {
            if (events.Any(e => string.IsNullOrWhiteSpace(e.Subject)))
            {
                var eventIds = string.Join(',', events.Where(e => string.IsNullOrWhiteSpace(e.Subject)).Select(e => e.Id));
                throw new NoEventSubjectException(eventIds);
            }
            if (events.Count() == 1)
            {
                await StoreEventAsync(events.First(), cancellationToken);
            }
            else
            {
                var container = GetContainer();
                var sortedEvents = events.OrderBy(e => e.Subject).ThenBy(e => e.SequenceNumber);
                var batchedEvents = new List<IEvent>();
                var subject = sortedEvents.First().Subject;
                foreach (var @event in sortedEvents)
                {
                    if (@event.Subject == subject)
                    {
                        batchedEvents.Add(@event);
                    }
                    else
                    {
                        if (batchedEvents.Count() == 1)
                        {
                            await StoreEventAsync(batchedEvents.First(), cancellationToken);
                        }
                        else
                        {
                            await StoreBatchedEventsAsync(container, batchedEvents, cancellationToken);
                        }
                        batchedEvents.Clear();
                        subject = @event.Subject;
                        batchedEvents.Add(@event);
                    }
                }
                if (batchedEvents.Count() > 0)
                {
                    await StoreBatchedEventsAsync(container, batchedEvents, cancellationToken);
                }
            }
        }
    }

    private async Task StoreBatchedEventsAsync(Container container, IEnumerable<IEvent> batchedEvents, CancellationToken cancellationToken = default)
    {
        var subject = batchedEvents.First().Subject;
        try
        {
            var partitionKey = new PartitionKey(subject);
            var batch = container.CreateTransactionalBatch(partitionKey);
            foreach (var batchedEvent in batchedEvents)
            {
                var jobject = (JObject)JToken.FromObject(batchedEvent);
                batch.CreateItem(jobject);
            }
            using (TransactionalBatchResponse batchResponse = await batch.ExecuteAsync(cancellationToken))
            {
                if (!batchResponse.IsSuccessStatusCode)
                {
                    throw new Exception("TransactionalBatch execution failed");
                }
            }
        }
        catch (CosmosException ex)
        {
            _logger.LogError($"Events with IDs: {String.Join(',', batchedEvents.Select(e => e.Id))} was not added successfully - error details: {ex.Message}");
            throw;
        }

    }

    public override async Task<IEnumerable<string>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var container = GetContainer();
            var queryDefinition = new QueryDefinition("SELECT c.partitionKey FROM c");
            FeedIterator<string> iterator = container.GetItemQueryIterator<string>(queryDefinition);
            List<string> partitionKeys = new();
            try
            {
                while (iterator.HasMoreResults)
                {
                    var feedResponse = await iterator.ReadNextAsync();
                    foreach (var partitionKey in feedResponse)
                    {
                        partitionKeys.Add(partitionKey);
                    }
                }
            }
            finally
            {
                if (iterator != null)
                {
                    iterator.Dispose();
                }
            }
            return partitionKeys;
        }
        catch (CosmosException ex)
        {
            _logger.LogError($"Entities was not retrieved successfully - error details: {ex.Message}");
            throw;
        }
    }

    private Container GetContainer()
    {
        var database = _client.GetDatabase(_configuration.DatabaseName);
        var container = database.GetContainer(_configuration.ContainerName);
        return container;
    }

    public void Dispose()
    {
        ((IDisposable)_client).Dispose();
    }
}
