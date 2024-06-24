using vesa.EventHub.Abstractions;

namespace vesa.EventHub.Infrastructure;

public class EventProcessorConfiguration : IEventProcessorConfiguration
{
    public string ConnectionStringKey { get; set; }
    public string ConsumerGroup { get; set; }
    public int MaximumRetries { get; set; } = 3;
    public int MaximumDelay { get; set; } = 1;
    public int TryTimeOut { get; set; } = 1;
    public string EventHubsRetryMode { get; set; } = "Exponential";
}
