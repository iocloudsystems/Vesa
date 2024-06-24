namespace eShop.Ordering.Data.Models;

public class Payment
{
    public string TransactionNumber { get; init; }
    public CreditCard CreditCard { get; init; }
    public decimal TotalAmount { get; init; }
}
