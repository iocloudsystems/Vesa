using eShop.Ordering.Data.Models;
using eShop.Ordering.Management.Application.Abstractions;

namespace eShop.Ordering.Management.Application.Infrastructure;

public class PaymentProcessor : IPaymentProcessor
{
    private static long _paymentNumber = 0;
    public Task<long> ProcessAsync(Payment payment) => Task.FromResult(++_paymentNumber);
}
