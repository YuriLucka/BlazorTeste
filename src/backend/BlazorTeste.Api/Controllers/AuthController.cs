using BlazorTeste.Api.Models;
using BlazorTeste.Application.Services.Interfaces;
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
