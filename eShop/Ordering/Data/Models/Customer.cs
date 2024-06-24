namespace eShop.Ordering.Data.Models;

public class Customer
{
    public Customer()
    {
    }

    public Customer(string customerNumber, string firstName, string lastName, string emailAddress)
    {
        CustomerNumber = customerNumber;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
    }

    public string CustomerNumber { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string EmailAddress { get; init; }
}
