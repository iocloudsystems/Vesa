using vesa.Core.Abstractions;
using vesa.Cosmos.Abstractions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace vesa.Cosmos.Infrastructure;

public class CosmosEventStoreListener : IEventStoreListener
{
    private readonly ChangeFeedProcessor _changeFeedProcessor;
    private readonly ILogger<CosmosEventStoreListener> _logger;

    public CosmosEventStoreListener
    (
        IChangeFeedProcessorFactory<JObject> changeFeedProcessorFactory,
        ILogger<CosmosEventStoreListener> logger
    )
    {
        _changeFeedProcessor = changeFeedProcessorFactory.CreateProcessor();
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _changeFeedProcessor.StartAsync();
    }

    public async Task StopAsync()
    {
        await _changeFeedProcessor.StopAsync();
    }
}
