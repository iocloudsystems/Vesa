using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.File.Abstractions;

namespace vesa.File.Infrastructure;

internal class FileEventConsumer : EventConsumer, IFileEventConsumer
{
    protected readonly string _consumerId;
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);

    public FileEventConsumer
    (
        string path, 
        string consumerId,
        IEnumerable<IEventMapping> eventMappings
    )
       : base(eventMappings)
    {
        Path = path;
        _consumerId = consumerId;
    }

    public string Path { get; init; }

    public override async Task<IEnumerable<IEvent>> ConsumeEventsAsync(CancellationToken cancellationToken)
    {
        await _semaphoregate.WaitAsync();
        var events = new List<IEvent>();
        var eventFileNames = Directory.GetFiles(Path, "*.txt");
        var lastSequenceNumber = await GetSequenceNumberFromBookmarkAsync();
        foreach (var eventFileName in eventFileNames)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            var @event = await GetEventFromFileAsync(eventFileName);
            if (lastSequenceNumber != 0 && @event.SequenceNumber > lastSequenceNumber)
            {
                events.Add(@event);
            }
        }
        if (events.Count > 0)
        {
            events = events.OrderBy(e => e.SequenceNumber).ToList();
            lastSequenceNumber = events.LastOrDefault().SequenceNumber;
            await System.IO.File.WriteAllTextAsync(GetBookmarkFileName(), lastSequenceNumber.ToString(), cancellationToken);
        }
        _semaphoregate.Release();
        return events.AsEnumerable();
    }

    protected virtual async Task<IEvent> GetEventFromFileAsync(string fileName)
    {

        IEvent? storedEvent = null;
        var fullFilename = @$"{Path}\{fileName}";
        if (System.IO.File.Exists(fullFilename))
        {
            var eventJson = await System.IO.File.ReadAllTextAsync(fullFilename);
            storedEvent = HydrateEvent(eventJson);
        }
        return storedEvent;
    }

    private string GetEventFileName(IEvent @event) => $"{@event.Subject}_{@event.GetType().Name}_{@event.Id}.txt";

    private async Task<long> GetSequenceNumberFromBookmarkAsync()
    {
        long lastSequenceNumber = 0;
        var filePath = @$"{Path}\{GetBookmarkFileName}";
        if (System.IO.File.Exists(filePath))
        {
            lastSequenceNumber = long.Parse(await System.IO.File.ReadAllTextAsync(filePath));
        }
        return lastSequenceNumber;
    }

    private string GetBookmarkFileName() => $"{_consumerId}.bmk";
}