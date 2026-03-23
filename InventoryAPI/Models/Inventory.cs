namespace InventoryAPI.Models;

public class Inventory
{
    public int ProductId {get; set;}
    public string ProductName {get; set;}
    public int WarehouseId {get; set;}
    public string WarehouseName {get; set;}
    public int Amount {get; set;}
}