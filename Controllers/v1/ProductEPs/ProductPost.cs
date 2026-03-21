using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Azure;

namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpPost]
    public async Task<ActionResult> CreateNewProduct(
        CreateProductRequest newProduct
    )
    {        
        try
        {
            var product = await _productService.CreateNewProductAsync(newProduct);
            
            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product
            );
        } 
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}