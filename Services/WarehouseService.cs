using InventoryAPI.Data;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

public partial class WarehouseService
{
    private readonly InventoryDbContext _dbContext;

    public WarehouseService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}