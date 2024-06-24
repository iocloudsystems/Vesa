using eShop.Ordering.Management.Application.Abstractions;

namespace eShop.Ordering.Management.Application.Infrastructure;

public class OrderNumberGenerator : IOrderNumberGenerator
{
    public Task<string> GenerateOrderNumberAsync() => Task.FromResult(Guid.NewGuid().ToString());
}
