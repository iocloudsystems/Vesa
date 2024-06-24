namespace vesa.Core.Abstractions;

public interface IEventConsumer : IEventConsumerBase
{
    void MapEventType(string sourceType, string targetType);
}
