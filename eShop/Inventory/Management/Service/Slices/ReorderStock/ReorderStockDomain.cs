using vesa.Core.Abstractions;
using eShop.Inventory.Core.Events;

namespace eShop.Inventory.Management.Service.ReorderStock;

public class ReorderStockDomain : IDomain<ReorderStockCommand>
{
    public ReorderStockDomain()
    {
    }

    public Task<IEnumerable<IEvent>> ProcessAsync(ReorderStockCommand command, CancellationToken cancellationToken = default)
    {
        var events = new List<IEvent>();
        var @event = new StockReorderedEvent
        (
            command.OrderNumber,
            command.Items,
            command.TriggeredBy,
            command.Id
        );
        events.Add(@event);
        return Task.FromResult(events.AsEnumerable());
    }
}
