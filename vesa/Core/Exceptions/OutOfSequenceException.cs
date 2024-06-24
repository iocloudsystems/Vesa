namespace vesa.Core.Exceptions;

public class OutOfSequenceException : Exception
{
    public OutOfSequenceException(string eventId)
    {
        EventId = eventId;
    }

    public string EventId { get; init; }
}
