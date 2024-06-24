using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class ParquetStateViewStreamSerializer<TStateView> : ParquetStreamSerializer<TStateView>
    where TStateView : IStateView, new()
{
}
