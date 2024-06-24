namespace vesa.Core.Abstractions;

public interface IStateView : IHasSubject
{
    string Id { get; }
    DateTimeOffset StateViewDate { get; }
    int LastEventSequenceNumber { get; }
    IStateView ApplyEvent(IEvent @event);
}


public interface IStateView<TEvent> : IStateView
    where TEvent : IEvent
{
    string GetSubject(TEvent @event);

    IStateView ApplyEvent(TEvent @event);
}