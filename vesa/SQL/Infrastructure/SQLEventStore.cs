using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace vesa.SQL.Infrastructure;

public class SQLEventStore : EventStore, IEventStore
{
    private readonly DbContext _dbContext;

    public SQLEventStore
    (
        DbContext dbContext,
        ILogger<SQLEventStore> logger
    )
        : base(logger)
    {
        _dbContext = dbContext;
    }

    public async override Task<int> CountEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<EventJson>().CountAsync(e => e.Subject == subject, cancellationToken);
    }

    public async override Task<IEvent> GetEventAsync(string eventId, string subject, CancellationToken cancellationToken = default)
    {
        var eventJson = await _dbContext.Set<EventJson>().SingleOrDefaultAsync(e => e.Subject == subject && e.Id == eventId, cancellationToken);
        return eventJson?.Event;
    }

    public async override Task<IEnumerable<IEvent>> GetEventsAsync(string subject, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<EventJson>().Where(e => e.Subject == subject).Select(e => e.Event).OrderBy(e => e.SequenceNumber).ToListAsync(cancellationToken);
    }

    public async override Task<IEnumerable<string>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<EventJson>().Select(e => e.Subject).Distinct().ToListAsync(cancellationToken);
    }

    public async override Task<bool> IdempotencyCheckAsync(string subject, string eventTypeName, string idempotencyToken, CancellationToken cancellationToken = default)
    {
        return !(await _dbContext.Set<EventJson>().AnyAsync(e => e.Subject == subject && e.EventTypeName == eventTypeName && e.IdempotencyToken == idempotencyToken, cancellationToken));
    }

    public async override Task StoreEventAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        _dbContext.Add(new EventJson(@event));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async override Task StoreEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            _dbContext.Add(new EventJson(@event));
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
