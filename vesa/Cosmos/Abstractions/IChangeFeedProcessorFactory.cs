using Microsoft.Azure.Cosmos;

namespace vesa.Cosmos.Abstractions;

public interface IChangeFeedProcessorFactory<TChangedItem>
{
    ChangeFeedProcessorBuilder Builder { get; init; }
    ChangeFeedProcessor CreateProcessor();
}

