using System.Security.Claims;
using BlazorTeste.Api.Models;
using BlazorTeste.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTeste.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthAppService authService) : ControllerBase
{
    private const string CookieName = "refreshToken";

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest req)
    {
        var result = await authService.LoginAsync(req.Email, req.Senha);
        if (result is null) return Unauthorized("E-mail ou senha inválidos.");

        if (result.RequiresTwoFactor)
            return Accepted(new LoginResponse { RequiresTwoFactor = true, MfaToken = result.MfaToken });

        SetRefreshCookie(result.Auth!.RefreshToken, result.Auth.RefreshTokenExpiry);
        return Ok(new LoginResponse
        {
            AccessToken = result.Auth.AccessToken,
            Nome = result.Auth.Nome,
            Email = result.Auth.Email
        });
    }

    [HttpPost("login/2fa")]
    public async Task<ActionResult<LoginResponse>> LoginTwoFactor([FromBody] TwoFactorVerifyRequest req)
    {
        var result = await authService.VerifyTwoFactorAsync(req.MfaToken, req.Code);
        if (result is null) return BadRequest("Código inválido ou sessão de verificação expirada.");

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiry);
        return Ok(new LoginResponse
        {
            AccessToken = result.AccessToken,
            Nome = result.Nome,
            Email = result.Email
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh()
    {
        var cookie = Request.Cookies[CookieName];
        if (string.IsNullOrEmpty(cookie)) return Unauthorized();

        var result = await authService.RefreshAsync(cookie);
        if (result is null) return Unauthorized();

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiry);
        return Ok(new LoginResponse
        {
            AccessToken = result.AccessToken,
            Nome = result.Nome,
            Email = result.Email
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var cookie = Request.Cookies[CookieName];
        if (!string.IsNullOrEmpty(cookie))
            await authService.LogoutAsync(cookie);

        var isHttps = Request.IsHttps;
        Response.Cookies.Delete(CookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax
        });
        return NoContent();
    }

    [Authorize]
    [HttpPost("2fa/setup")]
    public async Task<ActionResult<TwoFactorSetupResponse>> SetupTwoFactor()
    {
        var setup = await authService.SetupTwoFactorAsync(CurrentUserId());
        return Ok(new TwoFactorSetupResponse
        {
            SharedKey = setup.SharedKey,
            QrCodePngBase64 = setup.QrCodePngBase64
        });
    }

    [Authorize]
    [HttpPost("2fa/enable")]
    public async Task<IActionResult> EnableTwoFactor([FromBody] TwoFactorCodeRequest req)
    {
        var ok = await authService.EnableTwoFactorAsync(CurrentUserId(), req.Code);
        return ok ? NoContent() : BadRequest("Código inválido.");
    }

    [Authorize]
    [HttpPost("2fa/disable")]
    public async Task<IActionResult> DisableTwoFactor([FromBody] TwoFactorCodeRequest req)
    {
        var ok = await authService.DisableTwoFactorAsync(CurrentUserId(), req.Code);
        return ok ? NoContent() : BadRequest("Código inválido.");
    }

    private Guid CurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(raw!);
    }

    private void SetRefreshCookie(string token, DateTime expiry)
    {
        var isHttps = Request.IsHttps;
        Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Expires = expiry
        });
    }
}
