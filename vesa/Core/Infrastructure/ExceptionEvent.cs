namespace vesa.Core.Infrastructure;

public abstract class ExceptionEvent : Event
{
    public ExceptionEvent()
    {
    }

    public ExceptionEvent
    (
        Exception exception,
        string triggeredBy,
        string idempotencyToken
    )
        : base(triggeredBy, idempotencyToken)
    {
        Exception = exception;
    }

    public Exception Exception { get; init; }
}
