using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Services;

namespace InventoryAPI.Controllers;

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