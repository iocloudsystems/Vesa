using vesa.Blob.Abstractions;
using vesa.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace vesa.File.Infrastructure;

public class BlobEventStoreListener : IEventStoreListener
{
    private CancellationToken _cancellationToken;
    private readonly IBlobStorageConfiguration _configuration;
    private readonly IEventConsumerBase _eventConsumer;
    private readonly ILogger<BlobEventStoreListener> _logger;

    public BlobEventStoreListener
    (
        IBlobStorageConfiguration configuration,
        IEventConsumerBase eventConsumer,
        ILogger<BlobEventStoreListener> logger
    )
        : base()
    {
        _configuration = configuration;
        _eventConsumer = eventConsumer;
        _logger = logger;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        while (!_cancellationToken.IsCancellationRequested)
        {
            var events = await _eventConsumer.ConsumeEventsAsync(cancellationToken);
            await Task.Delay(_configuration.PollingIntervalInMilliseconds);
        }
    }

    public Task StopAsync()
    {
        _cancellationToken = new CancellationToken(true);
        return Task.CompletedTask;
    }
}