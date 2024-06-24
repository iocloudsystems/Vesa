﻿using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Inquiry.Service.GetStatusOrders;

public class StatusOrdersStateViewUpdater : IEventHandler<OrderPlacedEvent>, IEventHandler<OrderCancelledEvent>, IEventHandler<OrderReturnedEvent>
{
    private readonly IStateViewStore<StatusOrdersStateView> _stateViewStore;

    public StatusOrdersStateViewUpdater(IStateViewStore<StatusOrdersStateView> stateViewStore)
    {
        _stateViewStore = stateViewStore;
    }

    public async Task HandleAsync(OrderPlacedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(StatusOrdersStateView.GetDefaultSubject(@event.OrderStatus), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderCancelledEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(StatusOrdersStateView.GetDefaultSubject(@event.OrderStatus), @event, cancellationToken);
    }

    public async Task HandleAsync(OrderReturnedEvent @event, CancellationToken cancellationToken)
    {
        await _stateViewStore.UpdateStateViewAsync(StatusOrdersStateView.GetDefaultSubject(@event.OrderStatus), @event, cancellationToken);
    }
}