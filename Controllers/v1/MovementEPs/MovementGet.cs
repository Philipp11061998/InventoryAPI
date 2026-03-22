using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class MovementsController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        try
        {
            var result = await _movementService.GetAllAsync();
            return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetMovementById(int id)
    {       
        try
        {
            var result = await _movementService.GetMovementByIdAsync(id);
            
            if(result == null) return NotFound();
            else return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}