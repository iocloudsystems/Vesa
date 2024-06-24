namespace eShop.Ordering.Data.Models;

public class Product
{
    public Product()
    {
    }

    public Product(string sKU, string name, decimal price)
    {
        SKU = sKU;
        Name = name;
        Price = price;
    }

    public string SKU { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
}
