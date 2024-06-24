using eShop.Ordering.Management.Application.Abstractions;
using eShop.Ordering.Management.Events;
using eShop.Ordering.Management.Exceptions;
using vesa.Core.Abstractions;

namespace eShop.Ordering.Management.Service.PlaceOrder;

public class PlaceOrderDomain : IDomain<PlaceOrderCommand>
{
    private readonly IOrderNumberGenerator _orderNumberGenerator;
    private readonly IInventoryChecker _inventoryChecker;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IDeliveryScheduler _deliveryScheduler;

    public PlaceOrderDomain
    (
        IOrderNumberGenerator orderNumberGenerator,
        IInventoryChecker inventoryChecker,
        IPaymentProcessor paymentProcessor,
        IDeliveryScheduler deliveryScheduler
    )
    {
        _orderNumberGenerator = orderNumberGenerator;
        _inventoryChecker = inventoryChecker;
        _paymentProcessor = paymentProcessor;
        _deliveryScheduler = deliveryScheduler;
    }

    public async Task<IEnumerable<IEvent>> ProcessAsync(PlaceOrderCommand command, CancellationToken cancellationToken = default)
    {
        var events = new List<IEvent>();
        var orderNumber = await _orderNumberGenerator.GenerateOrderNumberAsync();

        var outOfStockItems = await _inventoryChecker.CheckOutOfStockItemsAsync(command.Items);
        if (outOfStockItems != null && outOfStockItems.Count() > 0)
        {
            events.Add(new OutOfStockExceptionEvent
            (
                new OutOfStockException(orderNumber, outOfStockItems),
                command.TriggeredBy,
                command.Id
            ));
        }
        else
        {
            var paymentTransactionNumber = await _paymentProcessor.ProcessAsync(command.Payment);
            if (paymentTransactionNumber < 0)
            {
                events.Add(new PaymentDeclinedExceptionEvent
                (
                    new PaymentDeclinedException(command.Payment.TransactionNumber, orderNumber),
                    command.TriggeredBy,
                    command.Id
                ));
            }
            else
            {
                var expectedDeliveryDate = await _deliveryScheduler.GetExpectedDeliveryDateAsync(command.Items, command.ShippingAddress);

                var orderPlacedEvent = new OrderPlacedEvent
                (
                    orderNumber,
                    command.Items,
                    command.CustomerNumber,
                    command.Customer,
                    command.ShippingAddress,
                    command.Payment.TransactionNumber,
                    command.Payment.CreditCard.Type,
                    command.Payment.TotalAmount,
                    expectedDeliveryDate,
                    command.TriggeredBy,
                    command.Id     // this IdempotencyToken prevents the command from being executed twice - there is a unique index on subject + IdempotencyToken
                );

                events.Add(orderPlacedEvent);
            }
        }

        return events;
    }
}
