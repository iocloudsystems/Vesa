using vesa.Core.Abstractions;
using System.Text.Json;

namespace vesa.File.Infrastructure;

public abstract class FileEventPublisherBase
{
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);
    public FileEventPublisherBase(string path)
    {
        Path = path;
    }

    public string Path { get; init; }

    protected async Task PublishCoreAsync(IEvent @event, string eventJson = null, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        if (string.IsNullOrWhiteSpace(eventJson))
        {
            eventJson = JsonSerializer.Serialize(@event);
        }
        var fileName = GetEventFileName(@event);
        if (string.IsNullOrWhiteSpace(Path))
        {
            await System.IO.File.WriteAllTextAsync(@$"{fileName}", eventJson, cancellationToken);
        }
        else
        {
            await System.IO.File.WriteAllTextAsync(@$"{Path}\{fileName}", eventJson, cancellationToken);
        }
        _semaphoregate.Release();
    }

    private string GetEventFileName(IEvent @event) => $"{@event.Subject}_{@event.GetType().Name}_{@event.Id}.txt";
}
