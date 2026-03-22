using InventoryAPI.Common;

namespace InventoryAPI.Models;

public class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Amount { get; set; }
    public MovementType MovementType { get; set; }
    public string? TransferReference { get; set; }
    public string? Note { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
}