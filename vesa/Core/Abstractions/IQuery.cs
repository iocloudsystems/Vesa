namespace vesa.Core.Abstractions;

public interface IQuery<TStateView>
    where TStateView : class, IStateView, new()
{
}
