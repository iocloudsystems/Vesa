namespace eShop.Ordering.Management.Exceptions;

public class PaymentDeclinedException : Exception
{
    public PaymentDeclinedException(string paymentTransactionNumber, string orderNumber)
    {
        PaymentTransactionNumber = paymentTransactionNumber;
        OrderNumber = orderNumber;
    }

    public string PaymentTransactionNumber { get; }
    public string OrderNumber { get; }
}
