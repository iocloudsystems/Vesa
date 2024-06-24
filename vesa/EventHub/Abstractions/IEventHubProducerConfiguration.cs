namespace vesa.EventHub.Abstractions;
public interface IEventHubProducerConfiguration
{
    string ConnectionStringKey { get; set; }
    int MaximumRetries { get; set; }
    int MaximumDelay { get; set; }
    int TryTimeOut { get; set; }
    string EventHubsRetryMode { get; set; }
}
