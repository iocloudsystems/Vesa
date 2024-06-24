using vesa.Kafka.Abstractions;

namespace vesa.Kafka.Infrastructure;

public class KafkaConsumerConfiguration : IKafkaConsumerConfiguration
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public string Topic { get; set; }
    public int BatchSize { get; set; } = 1;
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }

}
