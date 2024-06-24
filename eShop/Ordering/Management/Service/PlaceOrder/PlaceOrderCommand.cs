using vesa.Core.Infrastructure;
using eShop.Ordering.Data.Models;
using eShop.Ordering.Data.ValueObjects;

namespace eShop.Ordering.Management.Service.PlaceOrder;

public class PlaceOrderCommand : Command
{
    public PlaceOrderCommand
    (
        IList<OrderItem> items,
        string customerNumber,
        Customer? customer,
        Address shippingAddress,
        Payment payment,
        string triggeredBy,
        int lastEventSequenceNumber = 0
    )
        : base(triggeredBy, lastEventSequenceNumber)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
        CustomerNumber = customerNumber;
        Customer = customer;
        ShippingAddress = shippingAddress;
        Payment = payment;
    }

    public IList<OrderItem> Items { get; init; } = new List<OrderItem>();
    public string CustomerNumber { get; init; }
    public Customer? Customer { get; init; }
    public Address ShippingAddress { get; init; }
    public Payment Payment { get; init; }
}
