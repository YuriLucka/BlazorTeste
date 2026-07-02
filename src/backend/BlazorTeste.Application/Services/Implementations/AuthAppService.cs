using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QRCoder;

namespace BlazorTeste.Application.Services.Implementations;

public class AuthAppService(
    UserManager<ApplicationUser> userManager,
    IMemoryCache cache,
    IConfiguration config) : IAuthAppService
{
    private static readonly TimeSpan MfaTokenLifetime = TimeSpan.FromMinutes(5);

    public async Task<LoginResultDto?> LoginAsync(string email, string senha)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null) return null;

        if (await userManager.IsLockedOutAsync(user)) return null;

        if (!await userManager.CheckPasswordAsync(user, senha))
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        user.UltimoAcesso = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        if (user.TwoFactorEnabled)
        {
            var mfaToken = Guid.NewGuid().ToString("N");
            cache.Set($"mfa:{mfaToken}", user.Id, MfaTokenLifetime);
            return new LoginResultDto { RequiresTwoFactor = true, MfaToken = mfaToken };
        }

        return new LoginResultDto { Auth = await IssueTokensAsync(user) };
    }

    public async Task<AuthResultDto?> VerifyTwoFactorAsync(string mfaToken, string code)
    {
        if (!cache.TryGetValue($"mfa:{mfaToken}", out Guid userId)) return null;

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;

        if (await userManager.IsLockedOutAsync(user)) return null;

        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, code);

        if (!valid)
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);
        cache.Remove($"mfa:{mfaToken}");
        return await IssueTokensAsync(user);
    }

    public async Task<AuthResultDto?> RefreshAsync(string refreshToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null || user.RefreshTokenExpiry <= DateTime.UtcNow) return null;
        return await IssueTokensAsync(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await userManager.UpdateAsync(user);
    }

    public async Task<TwoFactorSetupDto> SetupTwoFactorAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new InvalidOperationException("Usuário não encontrado.");

        await userManager.ResetAuthenticatorKeyAsync(user);
        var key = await userManager.GetAuthenticatorKeyAsync(user)
            ?? throw new InvalidOperationException("Falha ao gerar chave do autenticador.");

        var uri = $"otpauth://totp/SindERP:{Uri.EscapeDataString(user.Email!)}" +
                  $"?secret={key}&issuer=SindERP&digits=6";

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        var png = qrCode.GetGraphic(10);

        return new TwoFactorSetupDto
        {
            SharedKey = key,
            QrCodePngBase64 = Convert.ToBase64String(png)
        };
    }

    public async Task<bool> EnableTwoFactorAsync(Guid userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;

        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, code);
        if (!valid) return false;

        await userManager.SetTwoFactorEnabledAsync(user, true);
        return true;
    }

    public async Task<bool> DisableTwoFactorAsync(Guid userId, string code)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return false;

        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, code);
        if (!valid) return false;

        await userManager.SetTwoFactorEnabledAsync(user, false);
        return true;
    }

    private async Task<AuthResultDto> IssueTokensAsync(ApplicationUser user)
    {
        var (refreshToken, expiry) = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = expiry;
        await userManager.UpdateAsync(user);

        return new AuthResultDto
        {
            AccessToken = GenerateAccessToken(user),
            Nome = user.Nome,
            Email = user.Email!,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = expiry
        };
    }

    private (string token, DateTime expiry) GenerateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiry = DateTime.UtcNow.AddDays(int.Parse(config["Jwt:RefreshTokenExpiryDays"]!));
        return (token, expiry);
    }

    private string GenerateAccessToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("name", user.Nome),
            new Claim("twoFactorEnabled", user.TwoFactorEnabled ? "true" : "false"),
            new Claim("permissoes", JsonSerializer.Serialize(user.Permissoes))
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
