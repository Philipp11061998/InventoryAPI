using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace InventoryAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public partial class MovementsController : ControllerBase
{
    private readonly MovementService _movementService;

    public MovementsController(MovementService movementService)
    {
        _movementService = movementService;
    }

    
}