using System.ComponentModel.DataAnnotations;
using InventoryAPI.Common;

namespace InventoryAPI.DTOs;

public class CreateMovementRequest
{

    [Required]
    public int ProductId {get; set;}
    
    [Required] 
    public int WarehouseId {get; set;}

    [Required, Range(1, int.MaxValue)]
    public int Amount {get; set;}
    
    [Required]
    public MovementType MovementType {get; set;}
    
    [MaxLength(100)]
    public string? TransferReference {get; set;} = null;
    
    [MaxLength(100)]
    public string? Note {get; set;} = null;
}