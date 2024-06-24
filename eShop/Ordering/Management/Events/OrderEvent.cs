using eShop.Ordering.Data.Enums;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Events;

public abstract class OrderEvent : Event
{
    public OrderEvent
    (
        string orderNumber,
        string customerNumber,
        OrderStatus orderStatus,
        decimal totalAmount,
        string triggeredBy,
        string idempotencyToken
    )
        : base(triggeredBy, idempotencyToken)
    {
        OrderNumber = orderNumber;
        if (!string.IsNullOrWhiteSpace(customerNumber))
        {
            CustomerNumber = customerNumber;
        }
        OrderStatus = orderStatus;
        TotalAmount = totalAmount;
    }

    public override string Subject => $"{SubjectPrefix}{OrderNumber ?? string.Empty}";
    public override string SubjectPrefix => "Order_";

    public string OrderNumber { get; init; }
    public string CustomerNumber { get; init; }
    public OrderStatus OrderStatus { get; init; }
    public decimal TotalAmount { get; init; }
}