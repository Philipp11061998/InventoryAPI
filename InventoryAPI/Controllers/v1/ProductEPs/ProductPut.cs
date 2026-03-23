using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;

namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProductById(
        int id,
        UpdateProductRequest product
    )
    {       
         var result = await _productService.UpdateProductByIdAsync(id, product);
            
        if(result == null) return NotFound();
        else return Ok(result);
    }
}