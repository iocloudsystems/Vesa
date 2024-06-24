using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.File.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace vesa.File.Infrastructure;

public class FileStateViewStore<TStateView> : StateViewStore<TStateView>, IFileStateViewStore<TStateView>
    where TStateView : class, IStateView, new()
{
    private const string STATE_VIEW_PATH_KEY = "StateViewPath";
    private readonly string _stateViewPath = default;
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);

    public FileStateViewStore
    (
        IConfiguration configuration,
        IEventStore eventStore,
        ILogger<FileStateViewStore<TStateView>> logger
    )
        : base(eventStore, logger)
    {
        Path = configuration[STATE_VIEW_PATH_KEY];
        var stateViewFolderName = typeof(TStateView).Name.Replace("StateView", "");
        _stateViewPath = string.IsNullOrWhiteSpace(Path) ? stateViewFolderName : @$"{Path}\{stateViewFolderName}";
        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
        }
        if (!Directory.Exists(_stateViewPath))
        {
            Directory.CreateDirectory(_stateViewPath);
        }
    }

    public string Path { get; init; }

    public override async Task<TStateView> GetStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        var stateViewFilePath = GetStateViewFilePath(subject);
        return await GetStateViewFromFileAsync(stateViewFilePath, cancellationToken);
    }

    public override async Task SaveStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        if (!@Directory.Exists(_stateViewPath))
        {
            Directory.CreateDirectory(_stateViewPath);
        }
        var stateViewFilePath = GetStateViewFilePath(stateView.Subject);
        var stateViewJson = JsonConvert.SerializeObject(stateView);
        await System.IO.File.WriteAllTextAsync(stateViewFilePath, stateViewJson, cancellationToken);
        _semaphoregate.Release();
    }

    public override async Task DeleteStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        if (!@Directory.Exists(_stateViewPath))
        {
            Directory.CreateDirectory(_stateViewPath);
        }
        var stateViewFilePath = GetStateViewFilePath(stateView.Subject);
        System.IO.File.Delete(stateViewFilePath);
        _semaphoregate.Release();
    }

    public override async Task DeleteStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        if (!@Directory.Exists(_stateViewPath))
        {
            Directory.CreateDirectory(_stateViewPath);
        }
        var stateViewFilePath = GetStateViewFilePath(subject);
        System.IO.File.Delete(stateViewFilePath);
        _semaphoregate.Release();
    }

    private async Task<TStateView> GetStateViewFromFileAsync(string filePath, CancellationToken cancellationToken)
    {
        TStateView? stateView = default(TStateView);
        if (System.IO.File.Exists(filePath))
        {
            var stateViewJson = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
            stateView = JsonConvert.DeserializeObject<TStateView>(stateViewJson);
        }
        return stateView;
    }

    private string GetStateViewFilePath(string subject) => @$"{_stateViewPath}\{subject}.txt";
}