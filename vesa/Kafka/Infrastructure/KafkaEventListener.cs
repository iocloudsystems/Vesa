using vesa.Core.Abstractions;
using vesa.Kafka.Abstractions;

namespace vesa.Kafka.Infrastructure;

public class KafkaEventListener : IEventListener, IDisposable
{
    protected CancellationToken _cancellationToken;
    private bool _disposedValue;
    protected readonly IKafkaEventConsumer _eventConsumer;
    protected readonly IEventProcessor _eventProcessor;

    public KafkaEventListener
    (
        IKafkaEventConsumer eventConsumer,
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
                _eventConsumer.UpdateCheckpoint(@event);
            }
        }
    }

    public Task StopAsync()
    {
        _cancellationToken = new CancellationToken(true);
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _eventConsumer.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
