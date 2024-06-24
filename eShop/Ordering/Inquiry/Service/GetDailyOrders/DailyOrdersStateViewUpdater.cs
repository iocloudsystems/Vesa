using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetDailyOrders;

public class DailyOrdersStateViewUpdater : IEventHandler<OrderPlacedEvent>, IEventHandler<OrderCancelledEvent>, IEventHandler<OrderReturnedEvent>
{
    private readonly IStateViewStore<DailyOrdersStateView> _stateViewStore;

    public DailyOrdersStateViewUpdater(IStateViewStore<DailyOrdersStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(DailyOrdersStateView.GetDefaultSubject(@event.EventDate), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(DailyOrdersStateView.GetDefaultSubject(@event.EventDate), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderReturnedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(DailyOrdersStateView.GetDefaultSubject(@event.EventDate), @event, cancellationToken);
    }
}
