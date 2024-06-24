using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public abstract class QueryHandler<TQuery, TStateView> : IQueryHandler<TQuery, TStateView>
    where TQuery : IQuery<TStateView>
    where TStateView : class, IStateView, new()

{
    private readonly IStateViewStore<TStateView> _stateViewStore;

    public QueryHandler(IStateViewStore<TStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public abstract Task<TStateView> HandleAsync(TQuery query);
}
