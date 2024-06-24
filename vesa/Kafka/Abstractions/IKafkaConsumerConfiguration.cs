namespace vesa.Kafka.Abstractions;

public interface IKafkaConsumerConfiguration
{
    string BootstrapServers { get; set; }
    string GroupId { get; set; }
    string Topic { get; set; }
    int BatchSize { get; set; }
    string SaslUsername { get; set; }
    string SaslPassword { get; set; }
}