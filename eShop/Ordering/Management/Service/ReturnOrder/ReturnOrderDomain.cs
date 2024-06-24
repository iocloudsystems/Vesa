using eShop.Ordering.Data.Enums;
using eShop.Ordering.Inquiry.StateViews;
using eShop.Ordering.Management.Events;
using eShop.Ordering.Management.Exceptions;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Management.Service.ReturnOrder;

public class ReturnOrderDomain : IDomain<ReturnOrderCommand, OrderStateView>
{
    private const int RETURN_PERIOD_IN_DAYS = 30;

    public ReturnOrderDomain()
    {
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(ReturnOrderCommand command, OrderStateView stateView, CancellationToken cancellation = default)
    {
        var events = new List<IEvent>();
        if (stateView.OrderStatus == OrderStatus.Shipped && stateView.DateOrdered.AddDays(RETURN_PERIOD_IN_DAYS) < DateTimeOffset.Now)
        {
            var orderReturnedExceptionEvent = new OrderNotReturnableExceptionEvent
            (
                new OrderNotReturnableException(command.OrderNumber),
                command.TriggeredBy,
                command.Id     // this IdempotencyToken prevents the command from being executed twice - there is a unique index on subject + IdempotencyToken
            );
            events.Add(orderReturnedExceptionEvent);
        }
        else
        {
            // Generate same event with different subjects
            var orderReturnedEvent = new OrderReturnedEvent
            (
                command.OrderNumber,
                stateView.CustomerNumber,
                stateView.TotalAmount,
                command.TriggeredBy,
                command.Id     // this IdempotencyToken prevents the command from being executed twice - there is a unique index on subject + IdempotencyToken
            );
            events.Add(orderReturnedEvent);
        }

        return await Task.FromResult(events.AsEnumerable());
    }
}