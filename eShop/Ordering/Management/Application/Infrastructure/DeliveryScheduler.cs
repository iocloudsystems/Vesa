using eShop.Ordering.Data.Models;
using eShop.Ordering.Data.ValueObjects;
using eShop.Ordering.Management.Application.Abstractions;

namespace eShop.Ordering.Management.Application.Infrastructure;

public class DeliveryScheduler : IDeliveryScheduler
{
    public Task<DateTimeOffset> GetExpectedDeliveryDateAsync(IEnumerable<OrderItem> items, Address shippingAddress) => Task.FromResult(DateTimeOffset.Now);
}
