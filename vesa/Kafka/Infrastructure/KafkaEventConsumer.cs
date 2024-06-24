using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Kafka.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace vesa.Kafka.Infrastructure;

public class KafkaEventConsumer : EventConsumer, IKafkaEventConsumer
{
    private IDictionary<IEvent, ConsumeResult<Ignore, string>> _results = new Dictionary<IEvent, ConsumeResult<Ignore, string>>();
    private readonly IKafkaConsumerConfiguration _configuration;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaEventConsumer> _logger;
    private bool _disposedValue;

    public KafkaEventConsumer
    (
        IKafkaConsumerConfiguration configuration,
        IConsumer<Ignore, string> consumer,
        ILogger<KafkaEventConsumer> logger,
        IEnumerable<IEventMapping> eventMappings
    )
        : base(eventMappings)
    {
        _configuration = configuration;
        _consumer = consumer;
        _logger = logger;
    }

    public override async Task<IEnumerable<IEvent>> ConsumeEventsAsync(CancellationToken cancellationToken)
    {

        var events = new List<IEvent>();
        try
        {
            //for (int i = 0; i < _configuration.BatchSize; i++)
            //{
            var consumeResult = _consumer.Consume(cancellationToken);
            var message = consumeResult?.Message?.Value;
            if (message != null)
            {
                var @event = HydrateEvent(message);
                events.Add(@event);
                _results.Add(@event, consumeResult);
            }
        }
        catch (ConsumeException ex)
        {

        }
        catch(OperationCanceledException ex2) 
        { 
        
        }
        //}
        return await Task.FromResult(events);
    }

    public void UpdateCheckpoint(IEvent @event)
    {
        var consumeResult = _results[@event];
        _consumer.StoreOffset(consumeResult);
        _results.Remove(@event);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _consumer.Close();
                _consumer.Dispose();
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
