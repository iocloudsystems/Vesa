namespace vesa.Core.Abstractions;

public interface IEventStore
{
    Task<IEnumerable<IEvent>> GetEventsAsync(string subject, CancellationToken cancellationToken = default);
    Task<int> CountEventsAsync(string subject, CancellationToken cancellationToken = default);
    Task<bool> EventExistsAsync(string eventId, string subject, CancellationToken cancellationToken = default);
    Task<bool> IdempotencyCheckAsync(string subject, string eventTypeName, string idempotencyToken, CancellationToken cancellationToken = default);
    Task AddEventAsync(IEvent @event, CancellationToken cancellationToken = default);
    Task StoreEventAsync(IEvent @event, CancellationToken cancellationToken = default);
    Task AddEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
    Task StoreEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetSubjectsAsync(CancellationToken cancellationToken = default);

}