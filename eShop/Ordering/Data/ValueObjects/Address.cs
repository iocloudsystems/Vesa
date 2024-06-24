namespace eShop.Ordering.Data.ValueObjects;

public class Address
{
    public Address()
    {
    }

    public Address(string unit, string streetNumber, string streetName, string city, string province, string postalCode)
    {
        Unit = unit;
        StreetNumber = streetNumber;
        StreetName = streetName;
        City = city;
        Province = province;
        PostalCode = postalCode;
    }

    public string Unit { get; init; }
    public string StreetNumber { get; init; }
    public string StreetName { get; init; }
    public string City { get; init; }
    public string Province { get; init; }
    public string PostalCode { get; init; }
    public override string ToString() => $"{Unit}{(string.IsNullOrWhiteSpace(Unit) ? string.Empty : "-")}{StreetNumber} {StreetName}, {City}, {Province} {PostalCode}";
}
