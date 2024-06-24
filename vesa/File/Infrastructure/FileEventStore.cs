using vesa.Core.Abstractions;
using vesa.Core.Constants;
using vesa.Core.Helpers;
using vesa.Core.Infrastructure;
using vesa.File.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace vesa.File.Infrastructure;

public class FileEventStore : EventStore, IFileEventStore
{
    static SemaphoreSlim _semaphoregate = new SemaphoreSlim(1);

    public FileEventStore
    (
        string path,
        ILogger<FileEventStore> logger
    )
        : base(logger)
    {
        Path = path;
        if (!Directory.Exists(Path))
        {
            Directory.CreateDirectory(Path);
        }
    }

    public string Path { get; init; }

    public override async Task<IEnumerable<IEvent>> GetEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        var events = new List<IEvent>();
        var partitionPath = GetPartitionPath(subject);
        if (Directory.Exists(partitionPath))
        {
            var eventFilePaths = Directory.GetFiles(partitionPath);
            foreach (var eventFilePath in eventFilePaths)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                var @event = await GetEventFromFileAsync(eventFilePath, cancellationToken);
                events.Add(@event);
            }
        }
        return events.OrderBy(e => e.SequenceNumber).AsEnumerable();
    }

    public override async Task<IEvent> GetEventAsync(string eventId, string subject, CancellationToken cancellationToken = default)
    {
        IEvent @event = default;
        if (Directory.Exists(GetPartitionPath(subject)))
        {
            var eventFilePath = GetEventFilePath(eventId, subject);
            @event = await GetEventFromFileAsync(eventFilePath, cancellationToken);
        }
        return @event;
    }

    public override async Task<int> CountEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        var partitionPath = GetPartitionPath(subject);
        return Directory.Exists(partitionPath) ? Directory.GetFiles(partitionPath).Length : 0;
    }

    public async Task<IEvent> GetEventFromFileAsync(string eventFilePath, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        IEvent? storedEvent = null;
        if (System.IO.File.Exists(eventFilePath) && eventFilePath.EndsWith(".txt"))
        {
            var eventJson = await System.IO.File.ReadAllTextAsync(eventFilePath);
            storedEvent = GetEventFromJson(eventJson);
        }
        _semaphoregate.Release();
        return storedEvent;
    }

    public override async Task<bool> IdempotencyCheckAsync(string subject, string eventTypeName, string idempotencyToken, CancellationToken cancellationToken = default)
    {
        var check = true;
        if (!string.IsNullOrWhiteSpace(idempotencyToken))
        {
            var partitionPath = GetPartitionPath(subject);
            if (Directory.Exists(partitionPath))
            {
                var eventFilePaths = Directory.GetFiles(partitionPath);
                foreach (var eventFilePath in eventFilePaths)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    var @event = await GetEventFromFileAsync(eventFilePath, cancellationToken);
                    if (@event.Subject == subject && @event.EventTypeName == eventTypeName && @event.IdempotencyToken == idempotencyToken)
                    {
                        check = false;
                        break;
                    }
                }
            }
        }
        return check;
    }

    public override async Task StoreEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        await _semaphoregate.WaitAsync();
        var partitionPath = GetPartitionPath(@event.Subject);
        if (!Directory.Exists(partitionPath))
        {
            Directory.CreateDirectory(partitionPath);
        }
        var eventFilePath = GetEventFilePath(@event.Id, @event.Subject);
        var eventJson = JsonConvert.SerializeObject(@event);
        await System.IO.File.WriteAllTextAsync(eventFilePath, eventJson, cancellationToken);
        _semaphoregate.Release();
    }

    public override async Task StoreEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            await StoreEventAsync(@event, cancellationToken);
        }
    }

    private IEvent GetEventFromJson(string eventJson)
    {
        var eventJObject = JsonConvert.DeserializeObject<JObject>(eventJson);
        var eventTypeName = (string)eventJObject[JsonPropertyName.EventTypeName];
        return (IEvent)JsonConvert.DeserializeObject(eventJson, TypeHelper.GetType(eventTypeName));
    }

    private string GetPartitionPath(string subject) => string.IsNullOrWhiteSpace(Path) ? subject : @$"{Path}\{subject}";

    private string GetEventFilePath(string eventId, string subject) => @$"{GetPartitionPath(subject)}\{eventId}.txt";

    public override Task<IEnumerable<string>> GetSubjectsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Directory.GetDirectories(Path).AsEnumerable());
}