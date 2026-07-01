using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Security;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlazorTeste.Application.Services.Implementations;

public class AuthAppService(
    IUsuarioRepository usuarioRepo,
    IPasswordHasher passwordHasher,
    IConfiguration config) : IAuthAppService
{
    public async Task<AuthResultDto?> LoginAsync(string email, string senha)
    {
        var user = await usuarioRepo.GetByEmailAsync(email.ToLower());
        if (user is null || !passwordHasher.Verify(senha, user.SenhaHash))
            return null;

        user.UltimoAcesso = DateTime.UtcNow;
        var (refreshToken, expiry) = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiry;
        await usuarioRepo.UpdateAsync(user);

        return new AuthResultDto
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = expiry
        };
    }

    public async Task<AuthResultDto?> RefreshAsync(string refreshToken)
    {
        var user = await usuarioRepo.GetByRefreshTokenAsync(refreshToken);
        if (user is null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return null;

        var (newToken, expiry) = GenerateRefreshToken();
        user.RefreshToken = newToken;
        user.RefreshTokenExpiry = expiry;
        await usuarioRepo.UpdateAsync(user);

        return new AuthResultDto
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email,
            RefreshToken = newToken,
            RefreshTokenExpiry = expiry
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await usuarioRepo.GetByRefreshTokenAsync(refreshToken);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await usuarioRepo.UpdateAsync(user);
    }

    private (string token, DateTime expiry) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(int.Parse(config["Jwt:RefreshTokenExpiryDays"]!));
        return (token, expiry);
    }

    private string GenerateAccessToken(Usuario user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Nome),
            new Claim("permissoes", System.Text.Json.JsonSerializer.Serialize(user.Permissoes))
        };
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:AccessTokenExpiryMinutes"]!)),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
