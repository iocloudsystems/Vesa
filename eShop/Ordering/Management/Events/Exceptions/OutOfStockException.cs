using eShop.Ordering.Data.Models;

namespace eShop.Ordering.Management.Exceptions;

public class OutOfStockException : Exception
{
    public OutOfStockException(string orderNumber, IEnumerable<OrderItem> outOfStockItems)
    {
        OrderNumber = orderNumber;
        if (outOfStockItems != null && outOfStockItems.Any())
        {
            foreach (var item in outOfStockItems)
            {
                Items.Add(item);
            }
        }
    }

    public string OrderNumber { get; init; }
    public IList<OrderItem> Items { get; } = new List<OrderItem>();
}
