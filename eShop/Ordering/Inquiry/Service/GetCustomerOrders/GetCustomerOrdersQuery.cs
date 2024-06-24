using eShop.Ordering.Inquiry.StateViews;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetCustomerOrders;

public class GetCustomerOrdersQuery : IQuery<CustomerOrdersStateView>
{
    public GetCustomerOrdersQuery(string customerNumber)
    {
        CustomerNumber = customerNumber;
    }

    public string CustomerNumber { get; }

    public static bool TryParse(string customerNumber, out GetCustomerOrdersQuery? query)
    {
        var parsed = false;
        query = null;
        if (!string.IsNullOrWhiteSpace(customerNumber))
        {
            query = new GetCustomerOrdersQuery(customerNumber);
            parsed = true;
        }
        return parsed;
    }
}
