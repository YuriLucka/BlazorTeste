using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IAuthAppService
{
    Task<LoginResultDto?> LoginAsync(string email, string senha);
    Task<AuthResultDto?> VerifyTwoFactorAsync(string mfaToken, string code);
    Task<AuthResultDto?> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
    Task<TwoFactorSetupDto> SetupTwoFactorAsync(Guid userId);
    Task<bool> EnableTwoFactorAsync(Guid userId, string code);
    Task<bool> DisableTwoFactorAsync(Guid userId, string code);
}
