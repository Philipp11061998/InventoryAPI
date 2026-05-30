using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace InventoryAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public partial class InventoryController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public InventoryController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    
}