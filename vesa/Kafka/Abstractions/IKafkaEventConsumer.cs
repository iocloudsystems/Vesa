using vesa.Core.Abstractions;

namespace vesa.Kafka.Abstractions;

public interface IKafkaEventConsumer : IEventConsumer, IDisposable
{
    void UpdateCheckpoint(IEvent @event);
}
