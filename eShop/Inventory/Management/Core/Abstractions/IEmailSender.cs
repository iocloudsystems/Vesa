namespace eShop.Inventory.Management.Core.Abstractions;
public interface IEmailSender
{
    Task SendAsync(IEmail email);
}