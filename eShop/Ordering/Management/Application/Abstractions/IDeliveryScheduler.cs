using eShop.Ordering.Data.Models;
using eShop.Ordering.Data.ValueObjects;

namespace eShop.Ordering.Management.Application.Abstractions;

public interface IDeliveryScheduler
{
    Task<DateTimeOffset> GetExpectedDeliveryDateAsync(IEnumerable<OrderItem> items, Address shippingAddress);
}
