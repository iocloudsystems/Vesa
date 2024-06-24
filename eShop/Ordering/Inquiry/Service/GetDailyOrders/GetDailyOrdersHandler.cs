using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetDailyOrders;

public class GetDailyOrdersHandler : IQueryHandler<GetDailyOrdersQuery, DailyOrdersStateView>
{
    private readonly IStateViewStore<DailyOrdersStateView> _stateViewStore;

    public GetDailyOrdersHandler(IStateViewStore<DailyOrdersStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task<DailyOrdersStateView> HandleAsync(GetDailyOrdersQuery query)
    {
        var subject = DailyOrdersStateView.GetDefaultSubject(query.StateViewDate);
        return await _stateViewStore.GetStateViewAsync(subject);
    }
}
