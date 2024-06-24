using eShop.Ordering.Data.Enums;
using eShop.Ordering.Data.Models;
using eShop.Ordering.Data.ValueObjects;
using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;
using vesa.Core.Exceptions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Inquiry.StateViews;

public class OrderStateView : StateView, IStateView<OrderPlacedEvent>, IStateView<OrderCancelledEvent>, IStateView<OrderReturnedEvent>
{
    public OrderStateView() : base()
    {
    }

    public OrderStateView
    (
        string orderNumber,
        DateTimeOffset dateOrdered,
        IEnumerable<OrderItem> items,
        string customerNumber,
        Customer customer,
        Address shippingAddress,
        string transactionNumber,
        CreditCardType creditCardType,
        decimal totalAmount,
        OrderStatus orderStatus,
        DateTimeOffset expectedDelivery,
        DateTimeOffset? dateCancelled,
        DateTimeOffset? dateReturned,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : base(stateViewDate, lastEventSequenceNumber)
    {
        OrderNumber = orderNumber ?? throw new ArgumentException($"{nameof(OrderNumber)} cannot be empty");
        if (!string.IsNullOrWhiteSpace(customerNumber))
        {
            CustomerNumber = customerNumber;
        }
        if (customer != null)
        {
            Customer = customer;
            CustomerNumber = Customer.CustomerNumber;
        }
        DateOrdered = dateOrdered;
        foreach (var item in items)
        {
            Items.Add(item);
        }
        Customer = customer;
        ShippingAddress = shippingAddress;
        TransactionNumber = transactionNumber;
        CreditCardType = creditCardType;
        TotalAmount = totalAmount;
        OrderStatus = orderStatus;
        ExpectedDelivery = expectedDelivery;
        DateCancelled = dateCancelled;
        DateReturned = dateReturned;
    }

    public OrderStateView
    (
        string id,
        string orderNumber,
        DateTimeOffset dateOrdered,
        IEnumerable<OrderItem> items,
        string customerNumber,
        Customer customer,
        Address shippingAddress,
        string transactionNumber,
        CreditCardType creditCardType,
        decimal totalAmount,
        OrderStatus orderStatus,
        DateTimeOffset expectedDelivery,
        DateTimeOffset? dateCancelled,
        DateTimeOffset? dateReturned,
        DateTimeOffset stateViewDate,
        int lastEventSequenceNumber
    )
        : this(orderNumber, dateOrdered, items, customerNumber, customer, shippingAddress, transactionNumber, creditCardType, totalAmount, orderStatus, expectedDelivery, dateCancelled, dateReturned, stateViewDate, lastEventSequenceNumber)
    {
        Id = id;
    }

    public static string GetDefaultSubject(string orderNumber) => $"Order_{orderNumber}";
    public override string Subject => GetDefaultSubject(OrderNumber ?? string.Empty);
    public override string SubjectPrefix => "Order_";

    public string OrderNumber { get; init; }
    public string CustomerNumber { get; init; }
    public DateTimeOffset DateOrdered { get; init; }
    public ICollection<OrderItem> Items { get; init; } = new List<OrderItem>();
    public Customer Customer { get; init; }
    public Address ShippingAddress { get; init; }
    public string TransactionNumber { get; init; }
    public CreditCardType CreditCardType { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus OrderStatus { get; private set; }
    public DateTimeOffset ExpectedDelivery { get; init; }
    public DateTimeOffset? DateCancelled { get; private set; }
    public DateTimeOffset? DateReturned { get; private set; }

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

    public string GetSubject(OrderPlacedEvent @event) => GetDefaultSubject(@event.OrderNumber);

    public IStateView ApplyEvent(OrderPlacedEvent @event) => new OrderStateView
    (
        Id,
        @event.OrderNumber,
        @event.EventDate,
        @event.Items,
        @event.CustomerNumber,
        @event.Customer,
        @event.ShippingAddress,
        @event.TransactionNumber,
        @event.CreditCardType,
        @event.TotalAmount,
        OrderStatus.Placed,
        @event.ExpectedDelivery,
        null,
        null,
        @event.EventDate,
        @event.SequenceNumber
    );

    public string GetSubject(OrderCancelledEvent @event) => GetDefaultSubject(@event.OrderNumber);

    public IStateView ApplyEvent(OrderCancelledEvent @event) => new OrderStateView
    (
        Id,
        @event.OrderNumber,
        DateOrdered,
        Items,
        @event.CustomerNumber,
        Customer,
        ShippingAddress,
        TransactionNumber,
        CreditCardType,
        TotalAmount,
        OrderStatus.Cancelled,
        ExpectedDelivery,
        @event.EventDate,
        DateReturned,
        @event.EventDate,
        @event.SequenceNumber
    );

    public string GetSubject(OrderReturnedEvent @event) => GetDefaultSubject(@event.OrderNumber);

    public IStateView ApplyEvent(OrderReturnedEvent @event) => new OrderStateView
    (
        Id,
        @event.OrderNumber,
        DateOrdered,
        Items,
        @event.CustomerNumber,
        Customer,
        ShippingAddress,
        TransactionNumber,
        CreditCardType,
        TotalAmount,
        OrderStatus.Returned,
        ExpectedDelivery,
        DateCancelled,
        @event.EventDate,
        @event.EventDate,
        @event.SequenceNumber
    );
}
