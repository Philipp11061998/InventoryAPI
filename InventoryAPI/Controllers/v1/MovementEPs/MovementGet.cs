using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class MovementsController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        var result = await _movementService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetMovementById(int id)
    {       
        var result = await _movementService.GetMovementByIdAsync(id);
        
        if(result == null) return NotFound();
        else return Ok(result);
    }
}