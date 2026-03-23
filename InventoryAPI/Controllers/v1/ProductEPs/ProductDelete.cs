using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;

namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProductById(
        int id    
    )
    {       
        var result = await _productService.DeleteProductByIdAsync(id);
        
        if(result == null) return NotFound();
        else return NoContent();
    }
}