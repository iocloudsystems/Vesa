using eShop.Ordering.Data.Models;
using eShop.Ordering.Management.Application.Abstractions;

namespace eShop.Ordering.Management.Application.Infrastructure;

public class InventoryChecker : IInventoryChecker
{
    public Task<IEnumerable<OrderItem>> CheckOutOfStockItemsAsync(IEnumerable<OrderItem> items) => Task.FromResult<IEnumerable<OrderItem>>(null);
}
