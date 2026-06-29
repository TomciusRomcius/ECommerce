namespace UserService.Domain.Models;

public class UpdateShippingAddressModel
{
    public required long ShippingAddressId { get; set; }
    public required string UserId { get; set; }
    public required string? RecipientName { get; set; }
    public required string? StreetAddress { get; set; }
    public required string? ApartmentUnit { get; set; }
    public required string? City { get; set; }
    public required string? State { get; set; }
    public required string? PostalCode { get; set; }
    public required string? Country { get; set; }
    public required string? MobileNumber { get; set; }
}