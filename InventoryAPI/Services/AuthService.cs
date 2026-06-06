using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryAPI.Data;
using InventoryAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using InventoryAPI.DTOs;
using System.Text.Json;

namespace InventoryAPI.Services;

public partial class AuthService
{
    private readonly InventoryDbContext _dbContext;
    private string _secret;
    private string _issuer;
    private string _audience;

    public AuthService(InventoryDbContext dbContext, string secret, string issuer, string audience)
    {
        _dbContext = dbContext;
        _secret = secret;
        _issuer = issuer;
        _audience = audience;
    }

    public async Task<UserToDisplay?> RegisterAsync(Register register)
    {
        if(await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == register.Username) != null)
        {
            return null;
        } 

        string passwordHash = GetPasswordHash(register.Password);

        User newUser = new User
        {
            Username = register.Username,
            PasswordHash = passwordHash
        };

        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();

        var insertedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == newUser.Username);

        return new UserToDisplay(insertedUser);

    }

    public async Task<string> LoginAsync(Login login)
    {
        User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

        if(user == null) throw new AuthenticationException("Bitte registriere dich zuerst!");

        if(ValidatePasswordHash(login.Password, user.PasswordHash))
        {
            return GenerateJwtToken(user);
        }
        else throw new AuthenticationException("Passwort falsch!");

    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    public string GetPasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool ValidatePasswordHash(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}