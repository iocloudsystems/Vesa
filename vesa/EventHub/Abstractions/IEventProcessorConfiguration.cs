namespace vesa.EventHub.Abstractions;
public interface IEventProcessorConfiguration
{
    string ConnectionStringKey { get; set; }
    string ConsumerGroup { get; set; }
    int MaximumRetries { get; set; }
    int MaximumDelay { get; set; }
    int TryTimeOut { get; set; }
    string EventHubsRetryMode { get; set; }
}
