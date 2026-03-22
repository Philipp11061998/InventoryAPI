using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

public partial class WarehouseService
{
    private readonly InventoryDbContext _dbContext;

    public WarehouseService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private WarehouseResponse MapToWarehouseResponse(Warehouse warehouse)
    {
        return new WarehouseResponse
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Description = warehouse.Description,
            CreatedAt = warehouse.created_at
        };
    }
}