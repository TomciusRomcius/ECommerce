namespace UserService.Presentation.Controllers.Address.dtos;

public class UpdateAddressDto
{
    public long ShippingAddressId { get; set; }
    public string? RecipientName { get; set; } = null;
    public string? StreetAddress { get; set; } = null;
    public string? ApartmentUnit { get; set; } = null;
    public string? City { get; set; } = null;
    public string? State { get; set; } = null;
    public string? PostalCode { get; set; } = null;
    public string? Country { get; set; } = null;
    public string? MobileNumber { get; set; } = null;
}