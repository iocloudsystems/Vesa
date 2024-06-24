using vesa.Core.Abstractions;

namespace vesa.Core.Infrastructure;

public abstract class StateView : IStateView
{
    protected StateView()
    {
    }

    protected StateView(DateTimeOffset stateViewDate, int lastEventSequenceNumber)
    {
        StateViewDate = stateViewDate;
        LastEventSequenceNumber = lastEventSequenceNumber;
    }

    protected StateView(string id, DateTimeOffset stateViewDate, int lastEventSequenceNumber)
    {
        Id = id;
        StateViewDate = stateViewDate;
        LastEventSequenceNumber = lastEventSequenceNumber;
    }

    public string Id { get; init; } = Guid.NewGuid().ToString();
    public abstract string Subject { get; }
    public abstract string SubjectPrefix { get; }
    public DateTimeOffset StateViewDate { get; init; } = DateTimeOffset.Now;
    public int LastEventSequenceNumber { get; init; }

    public abstract IStateView ApplyEvent(IEvent @event);

    protected bool CanApplyEvent(IEvent @event) => @event.SequenceNumber == LastEventSequenceNumber + 1;
}
