using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace InventoryAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public partial class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
}