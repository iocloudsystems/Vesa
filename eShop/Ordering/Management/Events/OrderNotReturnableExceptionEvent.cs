using eShop.Ordering.Management.Exceptions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Events;

public class OrderNotReturnableExceptionEvent : ExceptionEvent
{
    public OrderNotReturnableExceptionEvent
    (
        OrderNotReturnableException exception,
        string triggeredBy,
        string idempotencyToken
    )
        : base(exception, triggeredBy, idempotencyToken)
    {
    }

    public override string Subject => $"{SubjectPrefix}{(Exception as OrderNotReturnableException)?.OrderNumber ?? string.Empty}";
    public override string SubjectPrefix => "Order_";
}
