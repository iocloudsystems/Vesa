using eShop.Inventory.Data.Models;

namespace eShop.Inventory.Management.Core.Abstractions;

public interface ISupplier
{
    string SupplierNumber { get; init; }
    string Name { get; init; }
    string EmailAddress { get; init; }
    Task OrderStockAsync(StockOrder stockOrder);
}
