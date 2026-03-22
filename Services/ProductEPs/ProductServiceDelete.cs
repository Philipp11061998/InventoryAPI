using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial PUT Part
public partial class ProductService
{
    public async Task<ProductResponse?> DeleteProductByIdAsync(int id)
    {        
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

        if(product == null) return null;

        if(product.is_active == false) throw new InvalidOperationException("Product already inactive");

        product.is_active = false;

        await _dbContext.SaveChangesAsync();
        
        return ReturnProductResponse(product);
    }
}