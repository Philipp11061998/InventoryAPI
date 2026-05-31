using System.ComponentModel.DataAnnotations;
using InventoryAPI.Common;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class UsersController
{
    [HttpPatch("{id}/role")]
    public async Task<ActionResult> PatchUser(
        [FromRoute] int id,
        [FromQuery, Required] UserRoles role
    )
    {       
        var result = await _userService.PatchUserRoleAsync(id, role);
        if(result == null) return NotFound();
        return Ok(result);
    }

    [HttpPatch("{id}/soft-delete")]
    public async Task<ActionResult> SoftDeleteUser(
        [FromRoute] int id
    )
    {       
        var result = await _userService.SoftDeleteUserAsync(id);
        if(result == null) return NotFound();
        return Ok(result);
    }
}