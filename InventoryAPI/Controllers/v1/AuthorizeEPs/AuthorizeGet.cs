using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using InventoryAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class AuthController
{
    [HttpPost("login")]
    public async Task<ActionResult> Login(
        [FromBody, Required] Login login
    )
    {   
        try
        {
            var result = await _authService.LoginAsync(login.Username, login.Password);
            return Ok(result); 
        } 
        catch (AuthenticationException ex)
        {
            return Unauthorized(ex.Message);
        }
        
    }
}