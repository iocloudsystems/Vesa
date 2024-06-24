using vesa.Core.Infrastructure;

namespace eShop.Ordering.Administration.Events;
public class StateViewInstanceBuiltEvent : Event

{
    private readonly string _subject;

    public StateViewInstanceBuiltEvent
    (
        string stateViewName,
        string subject,
        string triggeredBy,
        string idempotencyToken
    )
        : base(triggeredBy, idempotencyToken)
    {
        StateViewName = stateViewName;
        _subject = subject;
    }

    public string StateViewName { get; }
    public override string Subject => $"{SubjectPrefix}{_subject}";
    public override string SubjectPrefix => "StateViewInstanceBuilt_";
}
