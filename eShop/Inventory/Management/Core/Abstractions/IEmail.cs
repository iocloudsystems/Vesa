namespace eShop.Inventory.Management.Core.Abstractions;

public interface IEmail
{
    string Body { get; init; }
    string From { get; init; }
    string Subject { get; init; }
    string To { get; init; }
}