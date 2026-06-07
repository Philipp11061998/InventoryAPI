using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using InventoryAPI.DTOs;
using InventoryAPI.Exceptions;
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
        try
        {
            UserToDisplay? result = await _authService.RegisterAsync(register);
            
            return CreatedAtAction(
                nameof(UsersController.GetUserById),
                "Users",
                new {id = result.Id},
                result
            ); 
        }
        catch(DomainException.UserAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }        
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
        catch (DomainException.UserNotFoundException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
}