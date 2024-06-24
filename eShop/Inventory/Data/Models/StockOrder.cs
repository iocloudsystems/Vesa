using eShop.Inventory.Data.ValueObjects;

namespace eShop.Inventory.Data.Models;

public class StockOrder
{
    public StockOrder(string supplierNumber, IEnumerable<OrderItem> items, string inventoryEmailAddress, DateTimeOffset dateOrdered)
    {
        SupplierNumber = supplierNumber;
        Items = items;
        InventoryEmailAddress = inventoryEmailAddress;
        DateOrdered = dateOrdered;
    }

    public string StockOrderNumber { get; } = Guid.NewGuid().ToString();
    public string SupplierNumber { get; init; }
    public IEnumerable<OrderItem> Items { get; } = new List<OrderItem>();
    public string InventoryEmailAddress { get; init; }
    public DateTimeOffset DateOrdered { get; init; }
}
