using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public abstract class Command : ICommand
{
    protected Command()
    {
    }

    protected Command(string triggeredBy, int lastSequenceNumber = 0)
    {
        TriggeredBy = triggeredBy;
        LastEventSequenceNumber = lastSequenceNumber;
    }

    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset CommandDate { get; init; } = DateTimeOffset.Now;
    public string TriggeredBy { get; init; }
    public int LastEventSequenceNumber { get; init; }
    public string Version { get; init; }
}
