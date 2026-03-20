using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Models;
namespace InventoryAPI.Controllers;

public partial class ProductsController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var result = await _repo.GetAll();
        if (!result.Success)
        {
            return StatusCode(500, result.Error);
        }

        return Ok(result);}
}