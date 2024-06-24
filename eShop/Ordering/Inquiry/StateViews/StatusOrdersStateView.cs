using eShop.Ordering.Data.Enums;
using eShop.Ordering.Data.Models;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;
using vesa.Core.Exceptions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Inquiry.StateViews;

public class StatusOrdersStateView : StateView, IStateView<OrderPlacedEvent>, IStateView<OrderCancelledEvent>, IStateView<OrderReturnedEvent>
{
    public StatusOrdersStateView() : base()
    {
    }

    public StatusOrdersStateView
    (
        OrderStatus orderStatus,
        IEnumerable<ShortOrder> orders,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : base(stateViewDate, lastEventSequenceNumber)
    {
        OrderStatus = orderStatus;
        foreach (var order in orders)
        {
            Orders.Add(order);
        }
    }

    public StatusOrdersStateView
    (
        string id,
        OrderStatus orderStatus,
        IEnumerable<ShortOrder> orders,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : this(orderStatus, orders, stateViewDate, lastEventSequenceNumber)
    {
        Id = id;
    }

    public static string GetDefaultSubject(OrderStatus orderStatus) => $"StatusOrders_{orderStatus.ToString()}";
    public override string Subject => GetDefaultSubject(OrderStatus);
    public override string SubjectPrefix => "StatusOrders_";

    public OrderStatus OrderStatus { get; init; }
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

    public string GetSubject(OrderPlacedEvent @event) => GetDefaultSubject(@event.OrderStatus);

    public IStateView ApplyEvent(OrderPlacedEvent @event) => ApplyOrderEvent(@event);

    public string GetSubject(OrderCancelledEvent @event) => GetDefaultSubject(@event.OrderStatus);

    public IStateView ApplyEvent(OrderCancelledEvent @event) => ApplyOrderEvent(@event);

    public string GetSubject(OrderReturnedEvent @event) => GetDefaultSubject(@event.OrderStatus);

    public IStateView ApplyEvent(OrderReturnedEvent @event) => ApplyOrderEvent(@event);

    private IStateView ApplyOrderEvent(OrderEvent @event)
    {
        IStateView stateView = null;
        //if (@event.OrderStatus == OrderStatus)
        //{
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
        stateView = new StatusOrdersStateView
        (
            Id,
            @event.OrderStatus,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
        //}
        return stateView;
    }
}