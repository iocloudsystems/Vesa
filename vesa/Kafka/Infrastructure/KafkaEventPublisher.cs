using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using vesa.Kafka.Abstractions;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace vesa.Kafka.Infrastructure
{
    public class KafkaEventPublisher : IEventPublisher
    {
        private readonly IKafkaPublisherConfiguration _configuration;
        private readonly IProducer<Ignore, string> _producer;

        public KafkaEventPublisher
        (
            IKafkaPublisherConfiguration configuration,
            IProducer<Ignore, string> producer,
            ILogger<KafkaEventPublisher> logger
        )
        {
            _configuration = configuration;
            _producer = producer;
        }

        public async Task PublishEventAsync(IEvent @event, CancellationToken cancellation = default)
        {
            var eventJson = JsonConvert.SerializeObject(@event);
            Message<Ignore, string> message = new() { Value = eventJson, Timestamp = new Timestamp(DateTime.Now) };
            await _producer.ProduceAsync(_configuration.Topic, message);
        }

        public async Task PublishCloudEventAsync<TEventData>(TEventData eventData, string source, string type = null)
               where TEventData : IEvent
        {
            var cloudEvent = new CloudEvent<TEventData>
            (
                eventData.Id.ToString(),
                source,
                type ?? eventData.GetType().Name,
                eventData.Subject,
                eventData.EventDate,
                eventData
            );
            var eventJson = JsonConvert.SerializeObject(cloudEvent);
            Message<Ignore, string> message = new() { Value = eventJson, Timestamp = new Timestamp(DateTime.Now) };
            await _producer.ProduceAsync(_configuration.Topic, message);
        }
    }
}
