using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.ChangeFeed;
using vesa.Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Configuration;

namespace vesa.Blob.Infrastructure;

//TODO: *** UNDER CONSTRUCTION: WAITING FOR .NET8 PRODUCTION RELEASE ***
public class BlobEventConsumer : IEventConsumerBase
{
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);
    private const string BLOB_EVENT_BOOKMARK_KEY = "BlobEventBookmark";
    private readonly BlobServiceClient _client;
    private readonly IStreamSerializer<IEvent> _eventStreamSerializer;
    private readonly IEventProcessor _eventProcessor;
    private readonly ILogger<BlobEventConsumer> _logger;

    public BlobEventConsumer
    (
        BlobServiceClient client,
        IStreamSerializer<IEvent> eventStreamSerializer,
        IEventProcessor eventProcessor,
        ILogger<BlobEventConsumer> logger
    )
    {
        _client = client;
        _eventStreamSerializer = eventStreamSerializer;
        _eventProcessor = eventProcessor;
        _logger = logger;
    }

    public async Task<IEnumerable<IEvent>> ConsumeEventsAsync(CancellationToken cancellationToken)
    {
        var events = new List<IEvent>();
        string bookmark = await GetBookmarkAsync();
        var changeFeedClient = _client.GetChangeFeedClient();
        var enumerator = changeFeedClient.GetChangesAsync(continuationToken: bookmark).AsPages().GetAsyncEnumerator();
        while (true)
        {
            var result = await enumerator.MoveNextAsync();
            if (result)
            {
                foreach (BlobChangeFeedEvent changeFeedEvent in enumerator.Current.Values)
                {
                    var urlParts = changeFeedEvent.EventData.Uri.AbsolutePath.Split('/');
                    var containerName = urlParts[urlParts.Count() - 3];
                    var subject = urlParts[urlParts.Count() - 2];
                    var fileName = urlParts.Last();
                    var filePath = @$"{subject}/{fileName}";
                    Stream stream = default;

                    var container = await GetBlobContainerAsync(containerName, cancellationToken);
                    var blob = container.GetBlobClient(filePath);
                    await blob.DownloadToAsync(stream, cancellationToken);
                    var @event = await _eventStreamSerializer.DeserializeAsync(stream);
                    events.Add(@event);

                    //TODO: if there are prior failures and we restart, we do NOT want to re-process events that have been processed
                    await _eventProcessor.ProcessAsync(@event, cancellationToken);
                }
                await SaveBookmarkAsync(enumerator.Current.ContinuationToken);
            }
            else
            {
                break;
            }

        }
        return events;
    }

    //TODO: improve implementation
    private async Task SaveBookmarkAsync(string bookmark)
    {
        await _semaphoregate.WaitAsync();
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        if (config.AppSettings.Settings.AllKeys.Contains(BLOB_EVENT_BOOKMARK_KEY))
        {
            config.AppSettings.Settings.Remove(BLOB_EVENT_BOOKMARK_KEY);
        }
        config.AppSettings.Settings.Add(BLOB_EVENT_BOOKMARK_KEY, bookmark);
        config.Save(ConfigurationSaveMode.Modified);
        _semaphoregate.Release();
    }

    //TODO: improve implementation
    private async Task<string> GetBookmarkAsync()
    {
        await _semaphoregate.WaitAsync();
        var bookmark = string.Empty;
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        if (config.AppSettings.Settings.AllKeys.Contains(BLOB_EVENT_BOOKMARK_KEY))
        {
            bookmark = config.AppSettings.Settings[BLOB_EVENT_BOOKMARK_KEY].Value;
        }
        _semaphoregate.Release();
        return bookmark;
    }

    private async Task<BlobContainerClient> GetBlobContainerAsync(string containerName, CancellationToken cancellationToken = default)
    {
        try
        {
            BlobContainerClient container = _client.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
            return container;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError($"Cannot find blob container: {containerName} - error details: {ex.Message}");
            throw;
        }
    }

}
