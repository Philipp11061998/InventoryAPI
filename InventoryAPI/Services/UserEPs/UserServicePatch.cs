using InventoryAPI.Common;
using InventoryAPI.Models;

namespace InventoryAPI.Services;

//Partial PATCH Part
public partial class UserService
{

    public async Task<User?> PatchUserRoleAsync(int userId, UserRoles role)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        if(user == null) return null;

        user.Role = role.ToString();

        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User?> SoftDeleteUserAsync(int userId)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        if(user == null) return null;

        user.IsActive = false;

        await _dbContext.SaveChangesAsync();

        return user;
    }
}