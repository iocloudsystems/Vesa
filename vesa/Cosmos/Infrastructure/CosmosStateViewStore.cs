using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Exceptions;
using vesa.Core.Infrastructure;
using vesa.Cosmos.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Cosmos.Infrastructure;

public class CosmosStateViewStore<TStateView> : StateViewStore<TStateView>, IDisposable
    where TStateView : class, IStateView, new()
{
    private readonly CosmosClient _client;
    private readonly ICosmosContainerConfiguration<IStateView> _configuration;

    public CosmosStateViewStore
    (
        CosmosClient client,
        ICosmosContainerConfiguration<IStateView> configuration,
        IEventStore eventStore,
        ILogger<CosmosStateViewStore<TStateView>> logger
    )
        : base(eventStore, logger)
    {
        _client = client;
        _configuration = configuration;
    }

    public override async Task<TStateView> GetStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        TStateView stateView = default;
        var container = GetContainer();
        using (var feedIterator = container.GetItemLinqQueryable<JObject>()
                .Where(s => (string)s[JsonPropertyName.Subject] == subject)
                .ToFeedIterator())
        {
            while (feedIterator.HasMoreResults)
            {
                try
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        stateView = (TStateView)JsonConvert.DeserializeObject(item.ToString(), typeof(TStateView));
                        break;
                    }
                }
                catch(Exception ex)
                {

                }
            }
            return stateView;
        }
    }

    public override async Task SaveStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(stateView.Subject))
        {
            throw new NoStateViewSubjectException();
        }

        try
        {
            var container = GetContainer();
            var partitionKey = new PartitionKey(stateView.Subject);
            var jobject = (JObject)JToken.FromObject(stateView);
            var itemResponse = await container.UpsertItemAsync(jobject, partitionKey, null, cancellationToken);
        }
        catch (CosmosException ex)
        {
            _logger.LogError($"New event with ID: {stateView.Subject} was not added successfully - error details: {ex.Message}");
            throw;
        }
    }

    public override async Task DeleteStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        await DeleteStateViewAsync(stateView.Subject, cancellationToken);
    }

    public override async Task DeleteStateViewAsync(string subject, CancellationToken cancellationToken)
    {
        var container = GetContainer();
        var partitionKey = new PartitionKey(subject);
        var requestOptions = new ItemRequestOptions { EnableContentResponseOnWrite = false };
        try
        {
            var stateView = await GetStateViewAsync(subject, cancellationToken);
            var response = await container.DeleteItemAsync<TStateView>(stateView.Id, partitionKey, requestOptions, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError($"Cosmos items don't exist for Subject={subject}, PartitionKey={partitionKey}");
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
        //((IDisposable)_client).Dispose();
    }
}

