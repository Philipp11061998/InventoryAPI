using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class ProductService
{

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        return await _dbContext.Products.Where(p => p.is_active)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Description = p.Description,
                CreatedAt = p.created_at
            })
            .ToListAsync();
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        Product? product = await _dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.is_active);
        
        return product == null ? null : ReturnProductResponse(product);
    }
}