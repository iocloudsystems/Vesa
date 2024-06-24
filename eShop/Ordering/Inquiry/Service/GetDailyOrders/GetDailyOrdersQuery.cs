using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetDailyOrders;

public class GetDailyOrdersQuery : IQuery<DailyOrdersStateView>
{
    public GetDailyOrdersQuery(DateTimeOffset stateViewDate)
    {
        StateViewDate = stateViewDate;
    }

    public DateTimeOffset StateViewDate { get; }

    public static bool TryParse(string stateViewDate, out GetDailyOrdersQuery? query)
    {
        var parsed = false;
        query = new GetDailyOrdersQuery(DateTimeOffset.Parse(stateViewDate));
        parsed = true;
        return parsed;
    }
}
