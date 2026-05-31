using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class UserController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetUserById(
        [FromRoute] int id
    )
    {
        var result = await _userService.GetUserByIdAsync(id);
        if(result == null) return NotFound();
        else return Ok(result);
    }
}