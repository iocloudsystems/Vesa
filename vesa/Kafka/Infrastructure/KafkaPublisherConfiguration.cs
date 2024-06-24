using vesa.Kafka.Abstractions;

namespace vesa.Kafka.Infrastructure
{
    public class KafkaPublisherConfiguration : IKafkaPublisherConfiguration
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }
    }
}
