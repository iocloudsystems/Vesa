using eShop.Inventory.Data.Models;

namespace eShop.Inventory.Data.ValueObjects;

public class OrderItem
{
    public OrderItem()
    {
    }

    public OrderItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    public Product Product { get; init; }
    public int Quantity { get; init; }
}
