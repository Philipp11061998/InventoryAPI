using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class ProductService
{

    public async Task<List<Product>> GetAllAsync()
    {
        return await _dbContext.Products.Where(p => p.is_active)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id && p.is_active);
        
        return product;
    }
}