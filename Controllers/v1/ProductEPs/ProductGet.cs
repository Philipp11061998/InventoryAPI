using Microsoft.AspNetCore.Mvc;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {       
        try
        {
            var result = await _productService.GetAllAsync();
            return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetProductById(int id)
    {       
        try
        {
            var result = await _productService.GetProductByIdAsync(id);
            
            if(result == null) return NotFound();
            else return Ok(result);
        } 
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}