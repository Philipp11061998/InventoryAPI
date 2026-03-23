using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Controllers;

public partial class WarehousesController
{
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateWarehouseById(
        int id,
        UpdateWarehouseRequest warehouse
    )
    {       
        var result = await _warehouseService.UpdateWarehouseByIdAsync(id, warehouse);
            
        if(result == null) return NotFound();
        else return Ok(result);
    }
}