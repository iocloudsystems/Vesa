using eShop.Ordering.Data.Models;

namespace eShop.Ordering.Management.Application.Abstractions
{
    public interface IPaymentProcessor
    {
        Task<long> ProcessAsync(Payment payment);
    }
}
