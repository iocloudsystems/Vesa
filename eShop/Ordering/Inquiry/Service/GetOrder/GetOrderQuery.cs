using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetOrder;

public class GetOrderQuery : IQuery<OrderStateView>
{
    public GetOrderQuery(string orderNumber)
    {
        OrderNumber = orderNumber;
    }

    public string OrderNumber { get; }

    public static bool TryParse(string orderNumber, out GetOrderQuery? query)
    {
        var parsed = false;
        query = null;
        if (!string.IsNullOrWhiteSpace(orderNumber))
        {
            query = new GetOrderQuery(orderNumber);
            parsed = true;
        }
        return parsed;
    }
}
