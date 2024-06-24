using eShop.Ordering.Data.Enums;

namespace eShop.Ordering.Data.Models;


public class ShortOrder
{
    public ShortOrder()
    {
    }

    public ShortOrder
    (
        string orderNumber,
        DateTimeOffset dateOrdered,
        string customerNumber,
        OrderStatus orderStatus,
        decimal totalAmount,
        DateTimeOffset? dateCancelled = null,
        DateTimeOffset? dateReturned = null
    )
    {
        OrderNumber = orderNumber;
        CustomerNumber = customerNumber;
        DateOrdered = dateOrdered;
        OrderStatus = orderStatus;
        TotalAmount = totalAmount;
        DateCancelled = dateCancelled;
        DateReturned = dateReturned;
    }

    public string OrderNumber { get; init; }
    public string CustomerNumber { get; init; }
    public DateTimeOffset DateOrdered { get; init; }

    public decimal TotalAmount { get; init; }
    public OrderStatus OrderStatus { get; private set; }
    public DateTimeOffset? DateCancelled { get; private set; }
    public DateTimeOffset? DateReturned { get; private set; }

    public void CancelOrder(DateTimeOffset dateCancelled)
    {
        OrderStatus = OrderStatus.Cancelled;
        DateCancelled = dateCancelled;
    }

    public void ReturnOrder(DateTimeOffset dateReturned)
    {
        OrderStatus = OrderStatus.Returned;
        DateReturned = dateReturned;
    }
}
