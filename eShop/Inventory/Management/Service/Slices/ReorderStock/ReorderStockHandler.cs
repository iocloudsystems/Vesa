using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;
using eShop.Inventory.Core.Events;
using eShop.Inventory.Core.IntegrationEvents;
using eShop.Inventory.Data.Models;
using eShop.Inventory.Management.Core.Abstractions;

namespace eShop.Inventory.Management.Service.ReorderStock;

public class ReorderStockHandler : CommandHandler<ReorderStockCommand>, IEventHandler<StockReorderedIntegrationEvent>, IEventHandler<StockReorderedEvent>
{
    private const string INVENTORY_EMAIL_ADDRESS = "inventory@eshop.com";
    private readonly ISupplier _supplier;

    public ReorderStockHandler
    (
        IServiceProvider serviceProvider,
        IDomain<ReorderStockCommand> domain,
        IEventStore eventStore,
        ISupplier supplier

    )
        : base(serviceProvider, domain, eventStore)
    {
        _supplier = supplier;
    }

    public async Task HandleAsync(StockReorderedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var reorderStockCommand = new ReorderStockCommand
        (
            // assign the event Id as the command Id so that when the domain generates an event,
            // it takes the command Id as the IdempotencyToken and will prevent the command from being handled twice
            @event.Id,
            @event.OrderNumber,
            @event.Items,
            @event.TriggeredBy,
            @event.SequenceNumber
        );
        await HandleAsync(reorderStockCommand, cancellationToken);
    }

    public async Task HandleAsync(StockReorderedEvent @event, CancellationToken cancellationToken)
    {
        var stockOrder = new StockOrder
        (
            _supplier.SupplierNumber,
            @event.Items,
            INVENTORY_EMAIL_ADDRESS,
            DateTimeOffset.Now
        );
        await _supplier.OrderStockAsync(stockOrder);
    }
}
