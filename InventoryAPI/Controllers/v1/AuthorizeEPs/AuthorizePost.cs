using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class AuthController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(
        [FromBody, Required] Register register
    )
    {   
        UserToDisplay? result = await _authService.RegisterAsync(register);
            
        if(result != null) return CreatedAtAction(
            nameof(UsersController.GetUserById),
            "Users",
            new {id = result.Id},
            result
        ); 
        else return BadRequest("Username bereits vorhanden");
        
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(
        [FromBody, Required] Login login
    )
    {   
        try
        {
            var result = await _authService.LoginAsync(login);
            return Ok(result); 
        } 
        catch (AuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
}