using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Service.ReturnOrder;

public class ReturnOrderCommand : Command
{
    public ReturnOrderCommand
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