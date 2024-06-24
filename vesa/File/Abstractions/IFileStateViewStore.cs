using vesa.Core.Abstractions;

namespace vesa.File.Abstractions;

public interface IFileStateViewStore<TStateView> : IStateViewStore<TStateView>
    where TStateView : class, IStateView
{
    string Path { get; }
}
