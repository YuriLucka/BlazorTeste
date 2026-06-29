using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorTeste.Api.Auth;
using BlazorTeste.Infrastructure.Data;
using BlazorTeste.Api.Models;
using Usuario = BlazorTeste.Domain.Entities.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BlazorTeste.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    private const string CookieName = "refreshToken";

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var user = await db.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == req.Email.ToLower());

        if (user is null || !PasswordHelper.Verify(req.Senha, user.SenhaHash))
            return Unauthorized("E-mail ou senha inválidos.");

        user.UltimoAcesso = DateTime.UtcNow;
        await SetRefreshTokenAsync(user);
        return Ok(new LoginResponse
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh()
    {
        var cookie = Request.Cookies[CookieName];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var user = await db.Usuarios.FirstOrDefaultAsync(u =>
            u.RefreshToken == cookie && u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user is null) return Unauthorized();

        await SetRefreshTokenAsync(user);
        return Ok(new LoginResponse
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies[CookieName];
        if (!string.IsNullOrEmpty(cookie))
        {
            var user = await db.Usuarios.FirstOrDefaultAsync(u => u.RefreshToken == cookie);
            if (user is not null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await db.SaveChangesAsync();
            }
        }
        Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
        return NoContent();
    }

    private async Task SetRefreshTokenAsync(Usuario user)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(config.GetValue<int>("Jwt:RefreshTokenExpiryDays"));
        user.RefreshToken = token;
        user.RefreshTokenExpiry = expiry;
        await db.SaveChangesAsync();
        Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiry
        });
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
            expires: DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:AccessTokenExpiryMinutes")),
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
