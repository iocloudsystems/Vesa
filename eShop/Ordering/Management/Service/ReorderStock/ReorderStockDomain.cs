using eShop.Ordering.Management.Events;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Management.Service.ReorderStock;

public class ReorderStockDomain : IDomain<ReorderStockCommand>
{
    public ReorderStockDomain()
    {
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(ReorderStockCommand command, CancellationToken cancellationToken = default)
    {
        var events = new List<IEvent>();
        var @event = new StockReorderedEvent
        (
            command.OrderNumber,
            command.Items,
            command.TriggeredBy,
            command.Id     // this IdempotencyToken prevents the command from being executed twice - there is a unique index on subject + IdempotencyToken
        );
        events.Add(@event);
        return await Task.FromResult(events.AsEnumerable());
    }
}
