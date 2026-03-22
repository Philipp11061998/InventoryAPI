using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class InventoryController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        try
        {
            var result = await _inventoryService.GetFullInventoryAsync();
            return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}