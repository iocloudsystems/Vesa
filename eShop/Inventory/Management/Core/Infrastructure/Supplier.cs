using eShop.Inventory.Data.Models;
using eShop.Inventory.Management.Core.Abstractions;
using Newtonsoft.Json;

namespace eShop.Inventory.Management.Core.Infrastructure;

public class Supplier : ISupplier
{
    private readonly IEmailSender _emailSender;

    public Supplier(string supplierNumber, string name, string emailAddress, IEmailSender emailSender)
    {
        SupplierNumber = supplierNumber;
        Name = name;
        EmailAddress = emailAddress;
        _emailSender = emailSender;
    }

    public string SupplierNumber { get; init; }
    public string Name { get; init; }
    public string EmailAddress { get; init; }

    public async Task OrderStockAsync(StockOrder stockOrder)
    {
        var email = new Email
        (
            stockOrder.InventoryEmailAddress,
            EmailAddress,
            $"Order No. {stockOrder.StockOrderNumber}",
            JsonConvert.SerializeObject(stockOrder)
        );
        await _emailSender.SendAsync(email);
    }
}
