using InventoryAPI.DTOs;
using InventoryAPI.Exceptions;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial POST Part
public partial class ProductService
{
    public async Task<ProductResponse> CreateNewProductAsync(CreateProductRequest newProduct)
    {
        var product = new Product
        {
            Sku = newProduct.Sku,
            Name = newProduct.Name,
            Description = newProduct.Description
        };

        if(await _dbContext.Products.AnyAsync(p => p.Sku == product.Sku))
        {
            throw new DomainException.ProductAlreadyExistsException(newProduct.Sku);
        }

        _dbContext.Products.Add(product);

        await _dbContext.SaveChangesAsync();

        return MapToProductResponse(product);
    }
}