namespace vesa.Core.Abstractions;

public interface IEventPropagationService<TEvent>
     where TEvent : class, IEvent
{
    IEnumerable<IEvent> GetPropagationEvents(TEvent @event);
}