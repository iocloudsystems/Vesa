using eShop.Inventory.Data.ValueObjects;
using vesa.Core.Infrastructure;

namespace eShop.Inventory.Management.Service.ReorderStock;

public class ReorderStockCommand : Command
{
    public ReorderStockCommand
    (
        string orderNumber,
        IEnumerable<OrderItem> items,
        string triggeredBy,
        int lastEventSequenceNumber
    )
        : base(triggeredBy, lastEventSequenceNumber)
    {
        OrderNumber = orderNumber;
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public ReorderStockCommand
    (
        string id,
        string orderNumber,
        IList<OrderItem> items,
        string triggeredBy,
        int lastEventSequenceNumber
    )
        : this(orderNumber, items, triggeredBy, lastEventSequenceNumber)
    {
        Id = id;
    }

    public string OrderNumber { get; init; }
    public IList<OrderItem> Items { get; init; } = new List<OrderItem>();
}
