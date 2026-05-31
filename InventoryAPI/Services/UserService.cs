using InventoryAPI.Data;
using InventoryAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

public partial class UserService
{
    private readonly InventoryDbContext _dbContext;

    public UserService(InventoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}