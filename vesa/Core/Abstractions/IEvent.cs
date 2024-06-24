namespace vesa.Core.Abstractions
{
    public interface IEvent : IHasSubject
    {
        string Id { get; }
        string EventTypeName { get; }
        DateTimeOffset EventDate { get; }
        string TriggeredBy { get; }
        int SequenceNumber { get; }
        string IdempotencyToken { get; }
        string Version { get; }
    }
}
