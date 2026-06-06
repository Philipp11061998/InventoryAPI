using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using BCrypt.Net;
using InventoryAPI.Common;
using InventoryAPI.Data;
using InventoryAPI.DTOs;
using InventoryAPI.Models;
using InventoryAPI.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPI.Tests.UnitTests;

public class AuthServiceTests
{
    private readonly string Secret = "vniHXX9ibZhosYpZ8gddlcfQbLCh0oYs7pcAwXBq17T"; //Normalerweise niemals als Klartext
    private readonly string Issuer = "portfolio-api";    
    private readonly string Audience = "portfolio-frontend";

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsValidUser()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        // Act
        var testUser = new Register{
            Username = "Test-User",
            Password = "Test-User123"
        };
        await authService.RegisterAsync(testUser);

        User? testUserFromDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == testUser.Username);

        //Assert
        Assert.NotNull(testUserFromDb);
        Assert.IsType<User>(testUserFromDb);
        Assert.Equal(testUser.Username, testUserFromDb.Username);
        Assert.True(BCrypt.Net.BCrypt.Verify(testUser.Password, testUserFromDb.PasswordHash));
        Assert.True(testUserFromDb.IsActive);
        Assert.Equal(UserRoles.User.ToString(), testUserFromDb.Role);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_SavePasswordAsHash()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        // Act
        var testUser = new Register{
            Username = "Test-User",
            Password = "Test-User123"
        };
        await authService.RegisterAsync(testUser);

        User? testUserFromDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == testUser.Username);

        //Assert
        Assert.NotNull(testUserFromDb);
        Assert.True(
            testUserFromDb.PasswordHash.StartsWith("$2a$") ||
            testUserFromDb.PasswordHash.StartsWith("$2b$")
        );

    }

    [Fact]
    public async Task RegisterAsync_AlreadyExistingUser_ReturnsNull()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        // Act
        var testUser = new Register{
            Username = user.Username,
            Password = "User123!"
        };
        var result = await authService.RegisterAsync(testUser);

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ValidRequest_ReturnsJwtToken()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        // Act
        var login = new Login
        {
            Username = user.Username,
            Password = "User123!"
        };

        var result = await authService.LoginAsync(login);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsAuthenticationException()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        // Act & Assert
        var login = new Login
        {
            Username = user.Username,
            Password = "FalschesPasswort123!"
        };

        AuthenticationException ex = await Assert.ThrowsAsync<AuthenticationException>(async () =>
        {
            await Task.WhenAll(
                authService.LoginAsync(login)
            );
        });

        Assert.Contains("Passwort falsch!", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_NonExistingUser_ThrowsAuthenticationException()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        // Act & Assert
        var login = new Login
        {
            Username = "Falscher Username",
            Password = "IrrelevantesPasswort123!"
        };

        AuthenticationException ex = await Assert.ThrowsAsync<AuthenticationException>(async () =>
        {
            await authService.LoginAsync(login);
        });


        Assert.Contains("Bitte registriere dich zuerst!", ex.Message);
    }

    [Fact]
    public async Task GenerateJwtToken_ValidRequest_ReturnsValidToken()
    {
        // Arrange
        var (dbContext, authService, user, admin) = await CreateTestPreparations();

        var login = new Login
        {
            Username = user.Username,
            Password = "User123!"
        };

        // Act
        var token = await authService.LoginAsync(login);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);


        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == "sub").Value);
        Assert.Equal(user.Username, jwtToken.Claims.First(c => c.Type == "name").Value);
        Assert.Equal(user.Role, jwtToken.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value);
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
    }

    private async Task<(InventoryDbContext dbContext, AuthService authService, User user, User admin)> CreateTestPreparations()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new InventoryDbContext(options);
        var authService = new AuthService(dbContext, Secret, Issuer, Audience);

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

        dbContext.Users.Add(admin);

        await dbContext.SaveChangesAsync();

        return (dbContext, authService, user, admin);
    }
}