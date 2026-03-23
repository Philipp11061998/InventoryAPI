using Microsoft.AspNetCore.Mvc;

namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        var result = await _productService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetProductById(int id)
    {       
        var result = await _productService.GetProductByIdAsync(id);
            
        if(result == null) return NotFound();
        else return Ok(result);
    }
}