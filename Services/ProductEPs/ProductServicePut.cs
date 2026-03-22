using InventoryAPI.Models;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial PUT Part
public partial class ProductService
{
    public async Task<ProductResponse?> UpdateProductByIdAsync(int id, UpdateProductRequest productInput)
    {
        if(string.IsNullOrEmpty(productInput.Sku) && string.IsNullOrEmpty(productInput.Name)) throw new InvalidOperationException("No changes possible. All fields are null");
        
        Product? product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

        if(product == null) return null;

        if(product.is_active == false) throw new InvalidOperationException("Product inactive. No actions possible");

        if(string.IsNullOrEmpty(productInput.Sku))
        {
            //Check if new Sku already exists
            Product? productSkuSearch = await _dbContext.Products.FirstOrDefaultAsync(p => p.Sku == productInput.Sku && p.Id != id);
            if(productSkuSearch != null) throw new InvalidOperationException("Sku already exists. No change possible");
        }

        product.Name = productInput.Name == null ? product.Name : productInput.Name;
        product.Sku = productInput.Sku == null ? product.Sku : productInput.Sku;
        product.Description = productInput.Description == null ? product.Description : productInput.Description;

        await _dbContext.SaveChangesAsync();
        
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Description = product.Description,
            CreatedAt = product.created_at
        };
    }
}