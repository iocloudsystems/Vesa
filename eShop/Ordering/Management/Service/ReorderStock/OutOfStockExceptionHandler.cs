using eShop.Ordering.Management.Events;
using eShop.Ordering.Management.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Management.Service.ReorderStock;

public class OutOfStockExceptionHandler : IEventHandler<OutOfStockExceptionEvent>
{
    private readonly IServiceProvider _serviceProvider;

    public OutOfStockExceptionHandler
    (
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleAsync(OutOfStockExceptionEvent @event, CancellationToken cancellationToken)
    {
        var outOfStockException = @event.Exception as OutOfStockException;
        var reorderStockCommand = new ReorderStockCommand
        (
            // assign the event Id as the command Id so that when the domain generates an event,
            // it takes the command Id as the IdempotencyToken and will prevent the command from being handled twice
            @event.Id,
            outOfStockException.OrderNumber,
            outOfStockException.Items,
            @event.TriggeredBy,
            @event.SequenceNumber
        );
        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var commandHandler = serviceProvider.GetRequiredService<ICommandHandler<ReorderStockCommand>>();
                var events = await commandHandler.HandleAsync(reorderStockCommand, new CancellationToken());
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
