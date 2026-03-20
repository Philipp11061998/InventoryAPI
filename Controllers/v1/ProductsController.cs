using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Models;

namespace InventoryAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class ProductsController : ControllerBase
{
    public ProductRepository _repo;
    public ProductsController(ProductRepository productRepository)
    {
        _repo = productRepository;
    }
}