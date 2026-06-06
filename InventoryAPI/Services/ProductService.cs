using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;

namespace InventoryAPI.Services;

public partial class ProductService
{
    private readonly InventoryDbContext _dbContext;

    public ProductService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private ProductResponse MapToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            CreatedAt = product.CreatedAt
        };
    }
}