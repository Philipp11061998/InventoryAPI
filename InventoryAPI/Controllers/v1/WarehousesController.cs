using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Services;

namespace InventoryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class WarehousesController : ControllerBase
{
    private readonly WarehouseService _warehouseService;

    public WarehousesController(WarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    
}