namespace vesa.Core.Abstractions;

public interface IStateViewStore<TStateView>
    where TStateView : class, IStateView
{
    Task<TStateView> HydrateStateViewAsync(string subject, DateTimeOffset? upToDate = null, CancellationToken cancellationToken = default);
    Task<TStateView> GetStateViewAsync(string subject, CancellationToken cancellationToken = default);
    Task SaveStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default);
    Task DeleteStateViewAsync(TStateView stateView, CancellationToken cancellationToken = default);
    Task DeleteStateViewAsync(string subject, CancellationToken cancellationToken = default);
    Task<TStateView> UpdateStateViewAsync(string subject, IEvent @event, CancellationToken cancellationToken = default);
}