using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Service.CancelOrder;

public class CancelOrderCommand : Command
{
    public CancelOrderCommand
    (
        string orderNumber,
        string triggeredBy,
        int lastEventSequenceNumber       // should match the OrderStateView.LastEventSequenceNumber
    )
        : base(triggeredBy, lastEventSequenceNumber)
    {
        OrderNumber = orderNumber;
    }

    public string OrderNumber { get; init; }
}