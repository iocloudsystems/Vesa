using eShop.Ordering.Data.Enums;
using eShop.Ordering.Data.Models;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;
using vesa.Core.Exceptions;
using vesa.Core.Extensions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Inquiry.StateViews;

public class CustomerOrdersStateView : StateView, IStateView<OrderPlacedEvent>, IStateView<OrderCancelledEvent>, IStateView<OrderReturnedEvent>
{
    public CustomerOrdersStateView() : base()
    {
    }

    public CustomerOrdersStateView
    (
        string customerNumber,
        Customer? customer,
        IEnumerable<ShortOrder> orders,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : base(stateViewDate, lastEventSequenceNumber)
    {
        CustomerNumber = customerNumber ?? throw new ArgumentException($"{nameof(CustomerNumber)} cannot be empty");
        Customer = customer;
        foreach (var order in orders)
        {
            Orders.Add(order);
        }
    }

    public CustomerOrdersStateView
    (
        string id,
        string customerNumber,
        Customer? customer,
        IEnumerable<ShortOrder> orders,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : this(customerNumber, customer, orders, stateViewDate, lastEventSequenceNumber)
    {
        Id = id;
    }

    public static string GetDefaultSubject(string customerNumber) => $"CustomerOrders_{customerNumber}";
    public override string Subject => GetDefaultSubject(CustomerNumber ?? string.Empty);
    public override string SubjectPrefix => "CustomerOrders_";

    public string CustomerNumber { get; init; }
    public Customer Customer { get; init; }
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

    public string GetSubject(OrderPlacedEvent @event) => GetDefaultSubject(@event.CustomerNumber);

    public IStateView ApplyEvent(OrderPlacedEvent @event)
    {
        var orders = Orders.Clone<List<ShortOrder>>();
        var order = new ShortOrder
        (
            @event.OrderNumber,
            @event.EventDate,
            @event.CustomerNumber,
            OrderStatus.Placed,
            @event.TotalAmount
        );
        orders.Add(order);
        return new CustomerOrdersStateView
        (
            Id,
            @event.CustomerNumber,
            @event.Customer,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
    }

    public string GetSubject(OrderCancelledEvent @event) => GetDefaultSubject(@event.CustomerNumber);

    public IStateView ApplyEvent(OrderCancelledEvent @event)
    {
        var orders = Orders.Clone<List<ShortOrder>>();
        var order = orders.Single(o => o.OrderNumber == @event.OrderNumber);
        order.CancelOrder(@event.EventDate);
        return new CustomerOrdersStateView
        (
            Id,
            @event.CustomerNumber,
            this.Customer,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
    }

    public string GetSubject(OrderReturnedEvent @event) => GetDefaultSubject(@event.CustomerNumber);

    public IStateView ApplyEvent(OrderReturnedEvent @event)
    {
        var orders = Orders.Clone<List<ShortOrder>>();
        var order = orders.Single(o => o.OrderNumber == @event.OrderNumber);
        order.ReturnOrder(@event.EventDate);
        return new CustomerOrdersStateView
        (
            Id,
            @event.CustomerNumber,
            this.Customer,
            orders,
            @event.EventDate,
            @event.SequenceNumber
        );
    }
}