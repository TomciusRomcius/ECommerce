using System.ComponentModel.DataAnnotations;

namespace UserService.Presentation.Controllers.Address.dtos;

public class SetAddressDto
{
    // Billing - 0, Shipping - 1
    [Required] public bool IsShipping { get; set; }

    [Required] public required string RecipientName { get; set; }

    [Required] public required string StreetAddress { get; set; }

    public string? ApartmentUnit { get; set; }

    [Required] public required string City { get; set; }

    [Required] public required string State { get; set; }

    [Required] public required string PostalCode { get; set; }

    [Required] public required string Country { get; set; }

    [Required] public required string MobileNumber { get; set; }
}