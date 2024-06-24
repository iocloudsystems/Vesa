using vesa.Core.Abstractions;
using vesa.File.Abstractions;

namespace vesa.File.Infrastructure;

public class FileEventStoreListener : IEventStoreListener
{
    private CancellationToken _cancellationToken;
    private FileSystemEventHandler OnChangeDelegate { get; set; }
    private FileSystemWatcher _watcher = new FileSystemWatcher();

    private readonly IFileEventStore _eventStore;
    private readonly IEventProcessor _eventProcessor;

    public FileEventStoreListener
    (
        IFileEventStore eventStore,
        IEventProcessor eventProcessor
    )
        : base()
    {
        _eventStore = eventStore;
        _eventProcessor = eventProcessor;
        OnChangeDelegate = OnChanged;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        _watcher.Path = _eventStore.Path;
        _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        _watcher.Filter = "*.*";
        _watcher.Changed += new FileSystemEventHandler(OnChangeDelegate);
        _watcher.IncludeSubdirectories = true;
        _watcher.EnableRaisingEvents = true;
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        return Task.CompletedTask;
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        try
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                StopAsync().ConfigureAwait(false);
            }
            var storedEvent = _eventStore.GetEventFromFileAsync(e.FullPath).GetAwaiter().GetResult();
            if (storedEvent != null)
            {
                _eventProcessor.ProcessAsync(storedEvent, _cancellationToken).GetAwaiter().GetResult();
            }
        }
        catch(Exception ex)
        {

        }
    }
}
