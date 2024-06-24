using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Service.ReorderStock;

public class ReorderStockHandler : CommandHandler<ReorderStockCommand>
{
    private readonly IEventPublisher _eventPublisher;

    public ReorderStockHandler
    (
        IServiceProvider serviceProvider,
        IDomain<ReorderStockCommand> domain,
        IEventStore eventStore
    )
        : base(serviceProvider, domain, eventStore)
    {
    }

    public override async Task<IEnumerable<IEvent>> HandleAsync(ReorderStockCommand command, CancellationToken cancellationToken = default)
    {
        //if (string.IsNullOrWhiteSpace(command.OrderNumber))
        //{
        //    throw new ArgumentException("Missing order number");
        //}
        if (command.Items.Count() == 0)
        {
            throw new ArgumentException("Missing items");
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
