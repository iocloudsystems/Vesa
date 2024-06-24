using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public abstract class Event : IEvent
{
    protected Event()
    {
        EventTypeName = GetType().FullName;
    }

    protected Event(string triggeredBy, string idempotencyToken) : this()
    {
        TriggeredBy = triggeredBy;
        IdempotencyToken = idempotencyToken;
    }

    public string Id { get; init; } = Guid.NewGuid().ToString();
    public abstract string Subject { get; }
    public abstract string SubjectPrefix { get; }
    public string EventTypeName { get; init; }
    public DateTimeOffset EventDate { get; init; } = DateTimeOffset.Now;
    public string TriggeredBy { get; init; }
    public int SequenceNumber { get; init; } = 0;
    public string IdempotencyToken { get; init; }
    public string Version { get; init; } = "1.0";
}
