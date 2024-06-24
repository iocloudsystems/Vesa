using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetOrder;

public class GetOrderHandler : IQueryHandler<GetOrderQuery, OrderStateView>
{
    private readonly IStateViewStore<OrderStateView> _stateViewStore;

    public GetOrderHandler(IStateViewStore<OrderStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task<OrderStateView> HandleAsync(GetOrderQuery query)
    {
        var subject = OrderStateView.GetDefaultSubject(query.OrderNumber);
        return await _stateViewStore.GetStateViewAsync(subject);
    }
}
