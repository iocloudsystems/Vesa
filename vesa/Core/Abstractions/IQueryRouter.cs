namespace vesa.Core.Abstractions;

public interface IQueryRouter<TQuery, TStateView>
    where TQuery : IQuery<TStateView>
    where TStateView : class, IStateView, new()
{
}
