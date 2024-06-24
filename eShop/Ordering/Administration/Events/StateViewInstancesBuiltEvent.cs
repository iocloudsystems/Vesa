using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Events;

public class StateViewInstancesBuiltEvent : Event
{
    public StateViewInstancesBuiltEvent
    (
        string stateViewName,
        string triggeredBy,
        string idempotencyToken
    )
        : base(triggeredBy, idempotencyToken)
    {
        StateViewName = stateViewName;
    }

    public string StateViewName { get; }
    public override string Subject => $"{SubjectPrefix}{StateViewName}";
    public override string SubjectPrefix => "StateViewInstancesBuilt_";
}
