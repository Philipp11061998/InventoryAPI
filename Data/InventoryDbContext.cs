using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    //public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    //public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();
}