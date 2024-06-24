using vesa.Core.Abstractions;
using vesa.Core.Infrastructure;

namespace eShop.Ordering.Management.Service.PlaceOrder;

public class PlaceOrderHandler : CommandHandler<PlaceOrderCommand>
{
    public PlaceOrderHandler
    (
        IServiceProvider serviceProvider,
        IDomain<PlaceOrderCommand> domain,
        IEventStore eventStore
    )
        : base(serviceProvider, domain, eventStore)
    {
    }

    public override async Task<IEnumerable<IEvent>> HandleAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.CustomerNumber))
        {
            throw new ArgumentException("Missing customer number");
        }
        if (command.Items.Count() == 0)
        {
            throw new ArgumentException("Missing items");
        }
        return await base.HandleAsync(command, cancellationToken);
    }
}
