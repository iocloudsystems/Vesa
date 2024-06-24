using vesa.Core.Abstractions;

namespace vesa.Core.Extensions;

public static class EventExtensions
{
    public static IEnumerable<TEvent> GetStateViewFeederEvents<TEvent>(this TEvent @event, IEnumerable<IStateView<TEvent>> stateViewFeeders)
        where TEvent : class, IEvent
    {
        var events = new List<TEvent>();
        var subjects = @event.GetStateViewFeederSubjects(stateViewFeeders);
        if (subjects?.Count() > 0)
        {
            foreach (var subject in subjects)
            {
                var newEvent = @event.SetNewInstanceProperty("Subject", subject) as TEvent;
                events.Add(newEvent);
            }
        }
        return events;
    }

    public static IEnumerable<string> GetStateViewFeederSubjects<TEvent>(this TEvent @event, IEnumerable<IStateView<TEvent>> stateViewFeeders)
        where TEvent : IEvent
    {
        IEnumerable<string> stateViewFeederSubjects = new List<string>();
        if (stateViewFeeders?.Count() > 0)
        {
            // grab the state views feeders Subjects
            stateViewFeederSubjects = stateViewFeeders
                .Select(stateView => stateView.GetSubject(@event))
                .Where(subject => subject != @event.Subject)
                .ToList();
        }
        return stateViewFeederSubjects;
    }
}