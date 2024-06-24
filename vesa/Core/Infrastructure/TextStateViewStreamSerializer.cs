using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public class TextStateViewStreamSerializer<TStateView> : TextStreamSerializer<TStateView>
    where TStateView : IStateView
{
}
