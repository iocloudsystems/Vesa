namespace vesa.Core.Exceptions;

public class NoEventSubjectException : Exception
{
    public NoEventSubjectException(string eventId)
    {
        EventId = eventId;
    }

    public string EventId { get; init; }
}
