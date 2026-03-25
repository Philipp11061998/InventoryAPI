using InventoryAPI.Common;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task DeleteProductAsync_ProductAlreadyInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var (dbContext, product, productService, productResponse) = await CreateTestPreparations();

        // Act & Assert
        await productService.DeleteProductByIdAsync(product.Id); //Erstes Löschen um auf inaktiv zu setzen (keine eigene Datenbankaktion, sondern nutzen der bestehenden Infrastruktur)

        InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await productService.DeleteProductByIdAsync(product.Id);
        }); //Zweite Löschung wirft dann die Exception

        Assert.Contains("Product already inactive", ex.Message);

    }

    [Fact]
    public async Task DeleteProductAsync_ProductNotFound_ReturnNull()
    {
        // Arrange
        var (dbContext, product, productService, productResponse) = await CreateTestPreparations(); //Nur Nutzen um Service und DB aufzubauen

        // Act & Assert
        var result = await productService.DeleteProductByIdAsync(2); //Nicht vorhandene Id

        Assert.Null(result);

    }

    [Fact]
    public async Task DeleteProductByIdAsync_ActiveProduct_ReturnsResponseAndSetsIsActiveFalse()
    {
        // Arrange
        var (dbContext, product, productService, productResponse) = await CreateTestPreparations(); //Nur Nutzen um Service und DB aufzubauen

        // Act & Assert
        var result = await productService.DeleteProductByIdAsync(product.Id); //Nicht vorhandene Id
        var productSearch = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

        Assert.NotNull(result);
        Assert.False(productSearch?.is_active);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Sku, result.Sku);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Description, result.Description);
        Assert.Equal(product.created_at, result.CreatedAt);

    }

    [Fact]
    public async Task GetAllAsync_GetOnlyActiveProducts_ReturnsOnlyActiveProducts()
    {
        // Arrange
        var (dbContext, product, productService, productResponse) = await CreateTestPreparations(); //Nur Nutzen um Service und DB aufzubauen
        var newProduct = new CreateProductRequest
        {
            Name = "Test inaktiv",
            Sku = "test-inactive"
        };

        var createdProduct = await productService.CreateNewProductAsync(newProduct);
        await productService.DeleteProductByIdAsync(createdProduct.Id);

        await dbContext.SaveChangesAsync();


        // Act & Assert
        var result = await productService.GetAllAsync(); //Nicht vorhandene Id

        Assert.Single(result);
        Assert.Equal(product.Id, result.First().Id);
        Assert.Equal(product.Sku, result.First().Sku);
        Assert.Equal(product.Name, result.First().Name);
        Assert.Equal(product.Description, result.First().Description);
        Assert.DoesNotContain(result, p => p.Id == createdProduct.Id);
    }

    private async Task<(InventoryDbContext dbContext, Product product, ProductService productService, ProductResponse? productResponse)> CreateTestPreparations()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);

        await dbContext.Database.EnsureCreatedAsync();

        var product = new Product
        {
            Sku = "test-product",
            Name = "Test Produkt",
            Description = null,
            is_active = true
        };

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync();

        var productService = new ProductService(dbContext);

        ProductResponse? productResponse = await productService.GetProductByIdAsync(product.Id);

        return (dbContext, product, productService, productResponse);
    }
}