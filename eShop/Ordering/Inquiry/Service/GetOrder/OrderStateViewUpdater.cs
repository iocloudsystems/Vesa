using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetOrder;

public class OrderStateViewUpdater : IEventHandler<OrderPlacedEvent>, IEventHandler<OrderCancelledEvent>, IEventHandler<OrderReturnedEvent>
{
    private readonly IStateViewStore<OrderStateView> _stateViewStore;

    public OrderStateViewUpdater(IStateViewStore<OrderStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(OrderStateView.GetDefaultSubject(@event.OrderNumber), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(OrderStateView.GetDefaultSubject(@event.OrderNumber), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderReturnedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(OrderStateView.GetDefaultSubject(@event.OrderNumber), @event, cancellationToken);
    }
}