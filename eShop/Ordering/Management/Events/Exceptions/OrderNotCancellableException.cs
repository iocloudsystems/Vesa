namespace eShop.Ordering.Management.Exceptions;

public class OrderNotCancellableException : Exception
{
    public OrderNotCancellableException(string orderNumber)
    {
        OrderNumber = orderNumber;
    }

    public string OrderNumber { get; }
}
