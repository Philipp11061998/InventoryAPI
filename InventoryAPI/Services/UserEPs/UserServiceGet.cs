using InventoryAPI.Common;
using InventoryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Services;

//Partial GET Part
public partial class UserService
{

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<List<User>> GetAllWithRoleFilterAsync(UserRoles role)
    {
        var users = await _dbContext.Users.Where(u => u.Role == role.ToString()).ToListAsync();;

        return users;
    }
}