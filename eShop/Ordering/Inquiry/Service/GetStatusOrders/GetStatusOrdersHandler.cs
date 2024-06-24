using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetStatusOrders;

public class GetStatusOrdersHandler : IQueryHandler<GetStatusOrdersQuery, StatusOrdersStateView>
{
    private readonly IStateViewStore<StatusOrdersStateView> _stateViewStore;

    public GetStatusOrdersHandler(IStateViewStore<StatusOrdersStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task<StatusOrdersStateView> HandleAsync(GetStatusOrdersQuery query)
    {
        var subject = StatusOrdersStateView.GetDefaultSubject(query.OrderStatus);
        return await _stateViewStore.GetStateViewAsync(subject);
    }
}
