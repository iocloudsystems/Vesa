using eShop.Ordering.Data.Models;

namespace eShop.Ordering.Management.Application.Abstractions
{
    public interface IInventoryChecker
    {
        Task<IEnumerable<OrderItem>> CheckOutOfStockItemsAsync(IEnumerable<OrderItem> items);
    }
}
