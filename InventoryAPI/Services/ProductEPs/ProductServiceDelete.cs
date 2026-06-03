using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial DELETE Part
public partial class ProductService
{
    public async Task<ProductResponse?> DeleteProductByIdAsync(int id)
    {        
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

        if(product == null) return null;

        if(product.IsActive == false) throw new InvalidOperationException("Product already inactive");

        product.IsActive = false;

        await _dbContext.SaveChangesAsync();
        
        return MapToProductResponse(product);
    }
}