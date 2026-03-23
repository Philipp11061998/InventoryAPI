using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;

namespace InventoryAPI.Controllers;

public partial class MovementsController
{
    [HttpPost]
    public async Task<ActionResult> CreateNewMovement(
        CreateMovementRequest newMovement
    )
    {        
        var movement = await _movementService.CreateNewMovementAsync(newMovement);
            
        return CreatedAtAction(
            nameof(GetMovementById),
            new { id = movement.Id },
            movement
        );
    }
}