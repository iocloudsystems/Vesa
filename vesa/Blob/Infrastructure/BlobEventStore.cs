using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Helpers;
using vesa.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.Blob.Infrastructure;

public class BlobEventStore : EventStore, IEventStore
{
    private readonly BlobServiceClient _client;
    private readonly string _eventStoreBlobContainerName;
    private readonly IStreamSerializer<IEvent> _eventStreamSerializer;

    public BlobEventStore
    (
        BlobServiceClient client,
        IConfiguration configuration,
        IStreamSerializer<IEvent> eventStreamSerializer,
        ILogger<BlobEventStore> logger
    )
        : base(logger)
    {
        _client = client;
        _eventStoreBlobContainerName = configuration.GetValue<string>("EventStoreBlobContainerName", "domain-events");
        _eventStreamSerializer = eventStreamSerializer;
    }

    public override async Task<IEnumerable<IEvent>> GetEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        var events = new List<IEvent>();

        var container = await GetBlobContainerAsync(_eventStoreBlobContainerName, cancellationToken);
        await foreach (BlobItem blobItem in container.GetBlobsAsync(prefix: @$"{subject}/", cancellationToken: cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            var blob = container.GetBlobClient(blobItem.Name);
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await blob.DownloadToAsync(memoryStream, cancellationToken);
                    memoryStream.Position = 0;
                    using (StreamReader streamReader = new StreamReader(memoryStream))
                    {
                        string result = streamReader.ReadToEnd();
                        var eventJObject = JsonConvert.DeserializeObject<JObject>(result);
                        var eventTypeName = (string)eventJObject[JsonPropertyName.EventTypeName];

                        var @event = (IEvent)JsonConvert.DeserializeObject(eventJObject.ToString(), TypeHelper.GetType(eventTypeName));

                        events.Add(@event);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        return events.OrderBy(e => e.SequenceNumber);
    }

    public override async Task<int> CountEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        int count = 0;
        var container = await GetBlobContainerAsync(_eventStoreBlobContainerName, cancellationToken);
        await foreach (BlobItem blobItem in container.GetBlobsAsync(prefix: @$"{subject}/", cancellationToken: cancellationToken))
        {
            count++;
        }
        return count;
    }

    public override async Task<IEvent> GetEventAsync(string eventId, string subject, CancellationToken cancellationToken = default)
    {
        Stream stream = default;
        var container = await GetBlobContainerAsync(_eventStoreBlobContainerName, cancellationToken);
        var filePath = GetEventFilePath(eventId, subject);
        var blob = container.GetBlobClient(filePath);
        await blob.DownloadToAsync(stream, cancellationToken);
        return await _eventStreamSerializer.DeserializeAsync(stream);
    }

    public override async Task<bool> IdempotencyCheckAsync(string subject, string eventTypeName, string idempotencyToken, CancellationToken cancellationToken = default)
    {
        var check = true;
        if (!string.IsNullOrWhiteSpace(idempotencyToken))
        {
            var container = await GetBlobContainerAsync(_eventStoreBlobContainerName, cancellationToken);
            await foreach (BlobItem blobItem in container.GetBlobsAsync(prefix: @$"{subject}/", cancellationToken: cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                var blob = container.GetBlobClient(blobItem.Name);
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await blob.DownloadToAsync(memoryStream, cancellationToken);
                        memoryStream.Position = 0;
                        using (StreamReader streamReader = new StreamReader(memoryStream))
                        {
                            string result = streamReader.ReadToEnd();
                            var eventJObject = JsonConvert.DeserializeObject<JObject>(result);
                            var blobEventTypeName = (string)eventJObject[JsonPropertyName.EventTypeName];
                            var blobIdempotencyToken = (string)eventJObject[JsonPropertyName.IdempotencyToken];
                            if (blobEventTypeName == eventTypeName && blobIdempotencyToken == idempotencyToken)
                            {
                                check = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
        return check;
    }

    public override async Task StoreEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var container = await GetBlobContainerAsync(_eventStoreBlobContainerName, cancellationToken);
        var filePath = GetEventFilePath(@event.Id, @event.Subject);
        var blob = container.GetBlobClient(filePath);
        var stream = await _eventStreamSerializer.SerializeAsync(@event);
        if (!await blob.ExistsAsync())
        {
            await blob.UploadAsync(stream, cancellationToken);
        }
    }

    public override async Task StoreEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await StoreEventAsync(@event, cancellationToken);
        }
    }

    private string GetEventFilePath(string eventId, string subject) => @$"{subject}/{eventId}.{_eventStreamSerializer.FileExtension}";

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

    public override async Task<IEnumerable<string>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        var container = await GetBlobContainerAsync(_eventStoreBlobContainerName, cancellationToken);
        var folders = container.GetBlobs().OfType<CloudBlobDirectory>();
        return folders.Select(f => f.Uri.ToString());
    }
}