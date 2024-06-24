using eShop.Ordering.Management.Exceptions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Events;

public class OrderNotCancellableExceptionEvent : ExceptionEvent
{
    public OrderNotCancellableExceptionEvent
    (
        OrderNotCancellableException exception,
        string triggeredBy,
        string idempotencyToken
    )
        : base(exception, triggeredBy, idempotencyToken)
    {
    }

    public override string Subject => $"{SubjectPrefix}{(Exception as OrderNotCancellableException)?.OrderNumber ?? string.Empty}";
    public override string SubjectPrefix => "Order_";

}

