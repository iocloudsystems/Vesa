using vesa.Core.Abstractions;
using vesa.File.Abstractions;

namespace vesa.File.Infrastructure;

public class FileEventHubListener : IEventHubListener
{
    protected CancellationToken _cancellationToken;
    protected readonly IFileEventConsumer _eventConsumer;
    protected readonly IEventProcessor _eventProcessor;

    public FileEventHubListener
    (
        IFileEventConsumer eventConsumer,
        IEventProcessor eventProcessor
    )
        : base()
    {
        _eventConsumer = eventConsumer;
        _eventProcessor = eventProcessor;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        while (!_cancellationToken.IsCancellationRequested)
        {
            var events = await _eventConsumer.ConsumeEventsAsync(cancellationToken);
            foreach (var @event in events)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                await _eventProcessor.ProcessAsync(@event, cancellationToken);
            }
        }
    }

    public Task StopAsync()
    {
        _cancellationToken = new CancellationToken(true);
        return Task.CompletedTask;
    }
}
