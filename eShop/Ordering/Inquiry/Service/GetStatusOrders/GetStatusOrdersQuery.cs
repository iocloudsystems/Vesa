using eShop.Ordering.Data.Enums;
using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetStatusOrders;

public class GetStatusOrdersQuery : IQuery<StatusOrdersStateView>
{
    public GetStatusOrdersQuery(OrderStatus orderStatus)
    {
        OrderStatus = orderStatus;
    }

    public OrderStatus OrderStatus { get; }

    public static bool TryParse(string orderStatusString, out GetStatusOrdersQuery? query)
    {
        if (Enum.TryParse(orderStatusString, out OrderStatus orderStatus))
        {
            query = new GetStatusOrdersQuery(orderStatus);
            return true;
        }
        else
        {
            query = null;
            return false;
        }
    }
}
