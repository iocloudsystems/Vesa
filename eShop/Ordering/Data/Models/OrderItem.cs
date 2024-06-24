namespace eShop.Ordering.Data.Models;

public class OrderItem
{
    public OrderItem()
    {
    }

    public OrderItem(string orderNumber, int itemNumber, Product product, int quantity)
    {
        OrderNumber = orderNumber;
        OrderNumber = orderNumber;
        Product = product;
        Quantity = quantity;
    }

    public string OrderNumber { get; init; }
    public int ItemNumber { get; init; }
    public Product Product { get; init; }
    public int Quantity { get; init; }
}
