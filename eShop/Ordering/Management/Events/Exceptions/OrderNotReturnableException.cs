namespace eShop.Ordering.Management.Exceptions;

public class OrderNotReturnableException : Exception
{
    public OrderNotReturnableException(string orderNumber)
    {
        OrderNumber = orderNumber;
    }

    public string OrderNumber { get; }
}
