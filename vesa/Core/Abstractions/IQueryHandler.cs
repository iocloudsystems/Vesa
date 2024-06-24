namespace vesa.Core.Abstractions;

public interface IQueryHandler<TQuery, TStateView>
    where TQuery : IQuery<TStateView>
    where TStateView : class, IStateView, new()
{
    Task<TStateView> HandleAsync(TQuery query);
}
