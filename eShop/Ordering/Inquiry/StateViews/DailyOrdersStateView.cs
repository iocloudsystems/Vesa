using eShop.Ordering.Data.Models;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;
using vesa.Core.Exceptions;
using vesa.Core.Extensions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Inquiry.StateViews;

public class DailyOrdersStateView : StateView, IStateView<OrderPlacedEvent>, IStateView<OrderCancelledEvent>, IStateView<OrderReturnedEvent>
{
    public DailyOrdersStateView() : base()
    {
    }

    public DailyOrdersStateView
    (
        IEnumerable<ShortOrder> orders,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : base(stateViewDate, lastEventSequenceNumber)
    {
        foreach (var order in orders)
        {
            Orders.Add(order);
        }
    }

    public DailyOrdersStateView
    (
        string id,
        IEnumerable<ShortOrder> orders,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : this(orders, stateViewDate, lastEventSequenceNumber)
    {
        Id = id;
    }

    public static string GetDefaultSubject(DateTimeOffset stateViewDate) => $"DailyOrders_{stateViewDate.ToString("yyyy-mm-dd")}";
    public override string Subject => GetDefaultSubject(StateViewDate);
    public override string SubjectPrefix => "DailyOrders_";

    public ICollection<ShortOrder> Orders { get; init; } = new List<ShortOrder>();

    public override IStateView ApplyEvent(IEvent @event)
    {
        dynamic typedEvent = Convert.ChangeType(@event, @event.GetType());
        if (CanApplyEvent(@event))
        {
            return ApplyEvent(typedEvent);
        }
        else
        {
            throw new OutOfSequenceException(@event.Id);
        }
    }

    public string GetSubject(OrderPlacedEvent @event) => GetDefaultSubject(@event.EventDate);

    public IStateView ApplyEvent(OrderPlacedEvent @event)
    {
        var orders = new List<ShortOrder>();
        orders.AddRange(Orders);
        var order = new ShortOrder
        (
            @event.OrderNumber,
            @event.EventDate,
            @event.CustomerNumber,
            @event.OrderStatus,
            @event.TotalAmount,
            @event.EventDate
        );
        orders.Add(order);
        return new DailyOrdersStateView
        (
            Id,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
    }

    public string GetSubject(OrderCancelledEvent @event) => GetDefaultSubject(@event.EventDate);

    public IStateView ApplyEvent(OrderCancelledEvent @event)
    {
        var orders = Orders.Clone<List<ShortOrder>>();
        var order = orders.Single(o => o.OrderNumber == @event.OrderNumber);
        order.CancelOrder(@event.EventDate);
        return new DailyOrdersStateView
        (
            Id,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
    }

    public string GetSubject(OrderReturnedEvent @event) => GetDefaultSubject(@event.EventDate);

    public IStateView ApplyEvent(OrderReturnedEvent @event)
    {
        var orders = Orders.Clone<List<ShortOrder>>();
        var order = orders.Single(o => o.OrderNumber == @event.OrderNumber);
        order.ReturnOrder(@event.EventDate);
        return new DailyOrdersStateView
        (
            Id,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
    }
}