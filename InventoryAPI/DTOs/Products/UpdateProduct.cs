using System.ComponentModel.DataAnnotations;
namespace InventoryAPI.DTOs;

//Getrennt von Create DTO, da langfristig andere Felder beim create nötig sein könnten
public class UpdateProductRequest
{
    [MaxLength(50)]
    public string? Sku { get; set; } = null; //Nur Update, wenn nicht null

    [MaxLength(50)]
    public string? Name { get; set; } = null; //Nur Update, wenn nicht null
   
    [MaxLength(100)]
    public string? Description { get; set; } = null; //Nur Update, wenn nicht null
}