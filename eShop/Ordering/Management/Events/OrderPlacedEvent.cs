using eShop.Ordering.Data.Enums;
using eShop.Ordering.Data.Models;
using eShop.Ordering.Data.ValueObjects;

namespace eShop.Ordering.Management.Events;

public class OrderPlacedEvent : OrderEvent
{
    public OrderPlacedEvent
    (
        string orderNumber,
        IEnumerable<OrderItem> items,
        string customerNumber,
        Customer? customer,
        Address shippingAddress,
        string transactionNumber,
        CreditCardType creditCardType,
        decimal totalAmount,
        DateTimeOffset expectedDelivery,
        string triggeredBy,
        string idempotencyToken
    )
        : base(orderNumber, customerNumber, OrderStatus.Placed, totalAmount, triggeredBy, idempotencyToken)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
        if (customer != null)
        {
            Customer = customer;
            CustomerNumber = Customer.CustomerNumber;
        }
        ShippingAddress = shippingAddress;
        TransactionNumber = transactionNumber;
        CreditCardType = creditCardType;
        ExpectedDelivery = expectedDelivery;
    }

    public IList<OrderItem> Items { get; init; } = new List<OrderItem>();
    public Customer? Customer { get; init; }
    public Address ShippingAddress { get; init; }
    public string TransactionNumber { get; init; }
    public CreditCardType CreditCardType { get; init; }
    public DateTimeOffset ExpectedDelivery { get; init; }
}