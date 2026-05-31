using System.ComponentModel.DataAnnotations;
using InventoryAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class UsersController
{
    [HttpGet]
    public async Task<ActionResult> GetAll(
        [FromQuery] UserRoles? role
    )
    {
        var result = role.HasValue
            ? await _userService.GetAllWithRoleFilterAsync(role.Value)
            : await _userService.GetAllUsersAsync();

        if(result == null) return NotFound();
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