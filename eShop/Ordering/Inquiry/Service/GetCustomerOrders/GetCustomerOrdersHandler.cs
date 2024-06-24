using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetCustomerOrders;

public class GetCustomerOrdersHandler : IQueryHandler<GetCustomerOrdersQuery, CustomerOrdersStateView>
{
    private readonly IStateViewStore<CustomerOrdersStateView> _stateViewStore;

    public GetCustomerOrdersHandler(IStateViewStore<CustomerOrdersStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task<CustomerOrdersStateView> HandleAsync(GetCustomerOrdersQuery query)
    {
        var subject = CustomerOrdersStateView.GetDefaultSubject(query.CustomerNumber);
        return await _stateViewStore.GetStateViewAsync(subject);
    }
}
