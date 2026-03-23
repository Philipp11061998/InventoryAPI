using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class InventoryController
{
    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] int? productId,
        [FromQuery] int? warehouseId
    )
    {       
        var result = await _inventoryService.GetFullInventoryAsync(productId, warehouseId);
        return Ok(result);
    }
}