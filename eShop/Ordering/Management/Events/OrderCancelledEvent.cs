using eShop.Ordering.Data.Enums;

namespace eShop.Ordering.Management.Events;

public class OrderCancelledEvent : OrderEvent
{
    public OrderCancelledEvent
    (
        string orderNumber,
        string customerNumber,
        decimal totalAmount,
        string triggeredBy,
        string idempotencyToken
    )
        : base(orderNumber, customerNumber, OrderStatus.Cancelled, totalAmount, triggeredBy, idempotencyToken)
    {
    }
}