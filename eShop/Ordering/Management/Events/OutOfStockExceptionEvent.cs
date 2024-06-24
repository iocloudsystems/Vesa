using eShop.Ordering.Management.Exceptions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Events;

public class OutOfStockExceptionEvent : ExceptionEvent
{
    public OutOfStockExceptionEvent
    (
        OutOfStockException exception,
        string triggeredBy,
        string idempotencyToken
    )
        : base(exception, triggeredBy, idempotencyToken)
    {
    }

    public override string Subject => $"{SubjectPrefix}{(Exception as OutOfStockException)?.OrderNumber ?? string.Empty}";
    public override string SubjectPrefix => "Order_";
}
