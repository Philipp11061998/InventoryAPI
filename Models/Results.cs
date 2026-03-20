namespace InventoryAPI.Models;

public class Results<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }
}