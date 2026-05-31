using InventoryAPI.Common;
using InventoryAPI.Data;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Tests;

public class UserServiceTest
{
    private readonly string Secret = "vniHXX9ibZhosYpZ8gddlcfQbLCh0oYs7pcAwXBq17T"; //Normalerweise niemals als Klartext
    private readonly string Issuer = "portfolio-api";    
    private readonly string Audience = "portfolio-frontend";

    [Fact]
    public async Task GetAllUsersAsync_ValidRequest_ReturnsAllUsers()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.GetAllUsersAsync();

        //Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }  

    [Fact]
    public async Task GetUserByIdAsync_ValidRequest_ReturnsUser()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.GetUserByIdAsync(user.Id);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<User>(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.Role, result.Role);
    }    

    [Fact]
    public async Task GetUserByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.GetUserByIdAsync(123123213);

        //Assert
        Assert.Null(result);
    }   

    [Fact]
    public async Task GetAllWithRoleFilterAsync_ValidRequest_ReturnsListOfFilteredUsers()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.GetAllWithRoleFilterAsync(UserRoles.User);

        //Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, u => Assert.Equal(UserRoles.User.ToString(), u.Role));
    }    

    [Fact]
    public async Task GetAllWithRoleFilterAsync_NoUserWithThisRole_ReturnsEmptyList()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        var userToBeAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username);

        userToBeAdmin!.Role = UserRoles.Admin.ToString();

        await dbContext.SaveChangesAsync();

        // Act
        var result = await userService.GetAllWithRoleFilterAsync(UserRoles.User);

        //Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }    

    [Fact]
    public async Task PatchUserRoleAsync_ValidRequest_ReturnsUserWithNewRole()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();
        var newRole = UserRoles.Admin;

        // Act
        var changedUser = await userService.PatchUserRoleAsync(user.Id, newRole);

        // Assert
        Assert.NotNull(changedUser);
        Assert.IsType<User>(changedUser);
        Assert.Equal(newRole.ToString(), changedUser.Role);
    }

    [Fact]
    public async Task PatchUserRoleAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();
        var newRole = UserRoles.Admin;

        // Act
        var changedUser = await userService.PatchUserRoleAsync(1231231, newRole);

        // Assert
        Assert.Null(changedUser);
    }

    [Fact]
    public async Task SoftDeleteUserAsync_ValidRequest_ReturnsDeletedUser()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.SoftDeleteUserAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<User>(result);
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task SoftDeleteUserAsync_ValidRequest_UserExistsInDb()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.SoftDeleteUserAsync(user.Id);
        var userExists = await dbContext.Users.AnyAsync(u => u.Username == user.Username);

        // Assert
        Assert.True(userExists);
    }

    [Fact]
    public async Task SoftDeleteUserAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var (dbContext, userService, user, admin) = await CreateTestPreparations();

        // Act
        var result = await userService.SoftDeleteUserAsync(123123213);

        // Assert
        Assert.Null(result);
    }
    
    private async Task<(InventoryDbContext dbContext, UserService userService, User user, User admin)> CreateTestPreparations()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);
        var authService = new AuthService(dbContext, Secret, Issuer, Audience);
        var userService = new UserService(dbContext);

        await dbContext.Database.EnsureCreatedAsync();

        var user = new User
        {
          Username = "User",
          PasswordHash =  authService.GetPasswordHash("User123!")
        };

        dbContext.Users.Add(user);

        var admin = new User
        {
            Username = "Admin",
            PasswordHash = authService.GetPasswordHash("Admin123!")
        };

        admin.Role = UserRoles.Admin.ToString();

        dbContext.Users.Add(admin);

        await dbContext.SaveChangesAsync();

        return (dbContext, userService, user, admin);
    }
}