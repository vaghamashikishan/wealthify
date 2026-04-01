using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using wealthify.DTOs.User;
using wealthify.Entity;
using wealthify.Extensions;
using wealthify.Repositories.Interfaces;
using wealthify.Services.Interfaces;

namespace wealthify.Services;

public class AuthService(IAuthRepository repository, IConfiguration config) : IAuthService
{
    public async Task RegisterAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = dto.ToEntity();
        user.Password = CreateHashPassword(user.Password);
        await repository.CreateAsync(user, cancellationToken);
    }

    public async Task<string?> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await repository.FindByEmail(dto.Email.ToLower(), cancellationToken);
        if (user is null || !VerifyPassword(dto.Password, user.Password))
        {
            return null;
        }
        return GenerateToken(user);
    }

    private static string CreateHashPassword(string password)
    {
        var hasher = new PasswordHasher<User>();
        return hasher.HashPassword(null!, password);
    }

    private static bool VerifyPassword(string password, string hashedPassword)
    {
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(null!, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SECRET_TOKEN"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["JWT:ISSUER"],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(config["JWT:EXPIRY_MINUTES"]!)),
            claims: claims,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

