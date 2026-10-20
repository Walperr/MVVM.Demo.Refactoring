namespace MVVM.Demo.Model;

public class Location
{
    public required string Country { get; init; }
    public required string State { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Street { get; init; }
    public required int BuildingNumber { get; init; }
    public required int OfficeNumber { get; init; }
}