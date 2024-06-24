using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace vesa.SQL.Infrastructure;

public class SQLStateViewStore<TStateViewJson, TStateView> : StateViewStore<TStateView>
    where TStateViewJson : StateViewJson<TStateView>, new()
    where TStateView : class, IStateView, new()
{
    private readonly DbContext _dbContext;

    public SQLStateViewStore
    (
        DbContext dbContext,
        IEventStore eventStore,
        ILogger<SQLStateViewStore<TStateViewJson, TStateView>> logger
    )
        : base(eventStore, logger)
    {
        _dbContext = dbContext;
    }

    public override async Task<TStateView> GetStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        var stateViewJson = await _dbContext.Set<TStateViewJson>().SingleOrDefaultAsync(e => e.Subject == subject, cancellationToken);
        return stateViewJson?.StateView;
    }

    public override async Task SaveStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        var storedStateViewJson = await _dbContext.Set<TStateViewJson>().SingleOrDefaultAsync(e => e.Subject == stateView.Subject, cancellationToken);
        if (storedStateViewJson == null)
        {
            var newStateViewJson = new TStateViewJson();
            newStateViewJson.StateView = stateView;
            _dbContext.Add(newStateViewJson);
        }
        else
        {
            storedStateViewJson.StateView = stateView;
            _dbContext.Update(storedStateViewJson);
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public override async Task DeleteStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default)
    {
        var storedStateViewJson = await _dbContext.Set<TStateViewJson>().SingleOrDefaultAsync(e => e.Subject == stateView.Subject, cancellationToken);
        if (storedStateViewJson != null)
        {
            _dbContext.Remove(storedStateViewJson);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public override async Task DeleteStateViewAsync(string subject, CancellationToken cancellationToken = default)
    {
        var storedStateViewJson = await _dbContext.Set<TStateViewJson>().SingleOrDefaultAsync(e => e.Subject == subject, cancellationToken);
        if (storedStateViewJson != null)
        {
            _dbContext.Remove(storedStateViewJson);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}