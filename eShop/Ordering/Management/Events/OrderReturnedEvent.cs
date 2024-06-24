using eShop.Ordering.Data.Enums;

namespace eShop.Ordering.Management.Events;

public class OrderReturnedEvent : OrderEvent
{
    public OrderReturnedEvent
    (
        string orderNumber,
        string customerNumber,
        decimal totalAmount,
        string triggeredBy,
        string idempotencyToken
    )
        : base(orderNumber, customerNumber, OrderStatus.Returned, totalAmount, triggeredBy, idempotencyToken)
    {
    }
}