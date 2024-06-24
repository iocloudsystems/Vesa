using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetCustomerOrders;

public class CustomerOrdersStateViewUpdater : IEventHandler<OrderPlacedEvent>, IEventHandler<OrderCancelledEvent>, IEventHandler<OrderReturnedEvent>
{
    private readonly IStateViewStore<CustomerOrdersStateView> _stateViewStore;

    public CustomerOrdersStateViewUpdater(IStateViewStore<CustomerOrdersStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(CustomerOrdersStateView.GetDefaultSubject(@event.CustomerNumber), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(CustomerOrdersStateView.GetDefaultSubject(@event.CustomerNumber), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderReturnedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(CustomerOrdersStateView.GetDefaultSubject(@event.CustomerNumber), @event, cancellationToken);
    }
}
