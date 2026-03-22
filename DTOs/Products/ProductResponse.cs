using System.ComponentModel.DataAnnotations;
namespace InventoryAPI.DTOs;

public class ProductResponse
{
    public int Id {get; set;}
    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
   
    public string? Description { get; set; }

    public DateTime CreatedAt {get; set;}
}