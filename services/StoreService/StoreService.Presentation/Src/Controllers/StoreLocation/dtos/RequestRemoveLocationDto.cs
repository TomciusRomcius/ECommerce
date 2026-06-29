using System.ComponentModel.DataAnnotations;

namespace StoreService.Presentation.Controllers.StoreLocation.dtos;

public class RequestRemoveLocationDto
{
    [Required] public int StoreLocationId { get; set; }
}