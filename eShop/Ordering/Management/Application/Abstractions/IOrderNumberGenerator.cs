namespace eShop.Ordering.Management.Application.Abstractions;

public interface IOrderNumberGenerator
{
    Task<string> GenerateOrderNumberAsync();
}
