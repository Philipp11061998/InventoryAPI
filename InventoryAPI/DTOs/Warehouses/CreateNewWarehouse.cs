using System.ComponentModel.DataAnnotations;
namespace InventoryAPI.DTOs;

public class CreateWarehouseRequest
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
   
    [MaxLength(100)]
    public string? Description { get; set; }
}