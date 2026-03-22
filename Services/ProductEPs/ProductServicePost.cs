using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace InventoryAPI.Services;

//Partial POST Part
public partial class ProductService
{
    public async Task<Product> CreateNewProductAsync(CreateProductRequest newProduct)
    {
        var product = new Product
        {
            Sku = newProduct.Sku,
            Name = newProduct.Name,
            Description = newProduct.Description
        };

        if(await _dbContext.Products.AnyAsync(p => p.Sku == product.Sku))
        {
            throw new InvalidOperationException("Product with the same SKU already exists.");
        }

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        return product;
    }
}