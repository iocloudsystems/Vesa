using eShop.Inventory.Management.Core.Abstractions;

namespace eShop.Inventory.Management.Core.Infrastructure;

public class Email : IEmail
{
    public Email(string from, string to, string subject, string body)
    {
        From = from;
        To = to;
        Subject = subject;
        Body = body;
    }

    public string From { get; init; }
    public string To { get; init; }
    public string Subject { get; init; }
    public string Body { get; init; }
}
