using vesa.Core.Infrastructure;
using eShop.Ordering.Data.Models;

namespace eShop.Ordering.Management.Service.ReorderStock;

public class ReorderStockCommand : Command
{
    //public ReorderStockCommand
    //(
    //    string orderNumber,
    //    IList<OrderItem> items,
    //    string triggeredBy,
    //    int lastEventSequenceNumber
    //)
    //    : base(triggeredBy, lastEventSequenceNumber)
    //{

    //}

    public ReorderStockCommand
    (
        string id,
        string orderNumber,
        IList<OrderItem> items,
        string triggeredBy,
        int lastEventSequenceNumber
    )
        : base(triggeredBy, lastEventSequenceNumber)
    {
        Id = id;
        OrderNumber = orderNumber;
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public string OrderNumber { get; init; }
    public IList<OrderItem> Items { get; init; } = new List<OrderItem>();
}
