using eShop.Ordering.Data.Enums;
using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using eShop.Ordering.Management.Exceptions;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Management.Service.CancelOrder;

public class CancelOrderDomain : IDomain<CancelOrderCommand, OrderStateView>
{
    public CancelOrderDomain()
    {
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(CancelOrderCommand command, OrderStateView stateView, CancellationToken cancellation = default)
    {
        var events = new List<IEvent>();
        if (stateView.OrderStatus != OrderStatus.Pending && stateView.OrderStatus != OrderStatus.Placed)
        {
            var orderCancelledExceptionEvent = new OrderNotCancellableExceptionEvent
            (
                new OrderNotCancellableException(command.OrderNumber),
                command.TriggeredBy,
                command.Id     // this IdempotencyToken prevents the command from being executed twice - there is a unique index on subject + IdempotencyToken
            );
            events.Add(orderCancelledExceptionEvent);
        }
        else
        {
            // Generate same event with different subjects
            var orderCancelledEvent = new OrderCancelledEvent
            (
                command.OrderNumber,
                stateView.CustomerNumber,
                stateView.TotalAmount,
                command.TriggeredBy,
                command.Id     // this IdempotencyToken prevents the command from being executed twice - there is a unique index on subject + IdempotencyToken
            );
            events.Add(orderCancelledEvent);
        }
        return await Task.FromResult(events.AsEnumerable());
    }
}