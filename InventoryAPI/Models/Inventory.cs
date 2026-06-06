namespace InventoryAPI.Models;

public class Inventory
{
    public int ProductId {get; set;}
    public required string ProductName {get; set;}
    public int WarehouseId {get; set;}
    public required string WarehouseName {get; set;}
    public int Amount {get; set;}
}