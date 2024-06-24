using eShop.Ordering.Data.Enums;

namespace eShop.Ordering.Data.Models;

public class CreditCard
{
    public string CardNumber { get; init; }
    public string CardHolderName { get; init; }
    public CreditCardType Type { get; init; }
    public DateTimeOffset Expiry { get; init; }
}
