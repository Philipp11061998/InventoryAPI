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
        try
        {
            var result = await _inventoryService.GetFullInventoryAsync(productId, warehouseId);
            return Ok(result);
        } 
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}