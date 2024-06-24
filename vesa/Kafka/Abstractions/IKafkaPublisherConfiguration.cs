
using Confluent.Kafka;

namespace vesa.Kafka.Abstractions;

public interface IKafkaPublisherConfiguration
{
    string BootstrapServers { get; set; }
    string Topic { get; set; }
    string SaslUsername { get; set; }
    string SaslPassword { get; set; }
}