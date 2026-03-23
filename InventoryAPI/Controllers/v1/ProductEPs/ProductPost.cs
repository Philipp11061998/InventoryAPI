using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;

namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpPost]
    public async Task<ActionResult> CreateNewProduct(
        CreateProductRequest newProduct
    )
    {        
        var product = await _productService.CreateNewProductAsync(newProduct);
            
        return CreatedAtAction(
            nameof(GetProductById),
            new { id = product.Id },
            product
        );
    }
}