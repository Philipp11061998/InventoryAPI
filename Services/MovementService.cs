using InventoryAPI.Data;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

public partial class MovementService
{
    private readonly InventoryDbContext _dbContext;

    public MovementService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}