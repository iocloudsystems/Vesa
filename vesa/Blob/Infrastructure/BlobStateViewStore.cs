using Azure;
using Azure.Storage.Blobs;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace vesa.Blob.Infrastructure;

public class BlobStateViewStore<TStateView> : StateViewStore<TStateView>
    where TStateView : class, IStateView, new()
{
    private readonly BlobServiceClient _client;
    private readonly string _stateViewBlobContainerName;
    private readonly IStreamSerializer<TStateView> _stateViewStreamSerializer;
    private readonly string _stateViewPath = default;

    public BlobStateViewStore
    (
        BlobServiceClient client,
        IConfiguration configuration,
        IStreamSerializer<TStateView> stateViewStreamSerializer,
        IEventStore eventStore,
        ILogger<BlobStateViewStore<TStateView>> logger
    )
        : base(eventStore, logger)
    {
        _client = client;
        _stateViewBlobContainerName = configuration.GetValue<string>("StateViewContainerName", "state-views");
        _stateViewStreamSerializer = stateViewStreamSerializer;
        _stateViewPath = typeof(TStateView).Name;
    }

    public override async Task<TStateView> GetStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        Stream stream = default;
        var container = await GetBlobContainerAsync(_stateViewBlobContainerName, cancellationToken);
        var fileName = GetStateViewFileName(subject, _stateViewPath);
        var blob = container.GetBlobClient(fileName);
        await blob.DownloadToAsync(stream, cancellationToken);
        return await _stateViewStreamSerializer.DeserializeAsync(stream);
    }

    public override async Task SaveStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        var container = await GetBlobContainerAsync(_stateViewBlobContainerName, cancellationToken);
        var fileName = GetStateViewFileName(stateView.Subject, _stateViewPath);
        var blob = container.GetBlobClient(fileName);
        var stream = await _stateViewStreamSerializer.SerializeAsync(stateView);
        await blob.UploadAsync(stream, cancellationToken);
    }

    public override async Task DeleteStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        var container = await GetBlobContainerAsync(_stateViewBlobContainerName, cancellationToken);
        var fileName = GetStateViewFileName(stateView.Subject, _stateViewPath);
        var blob = container.GetBlobClient(fileName);
        await blob.DeleteAsync(cancellationToken: cancellationToken);
    }

    public override async Task DeleteStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        var container = await GetBlobContainerAsync(_stateViewBlobContainerName, cancellationToken);
        var fileName = GetStateViewFileName(subject, _stateViewPath);
        var blob = container.GetBlobClient(fileName);
        await blob.DeleteAsync(cancellationToken: cancellationToken);
    }

    private string GetStateViewFileName(string subject, string path) => @$"{path}/{subject}.{_stateViewStreamSerializer.FileExtension}";

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