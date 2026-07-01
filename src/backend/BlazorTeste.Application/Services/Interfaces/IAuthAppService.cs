using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IAuthAppService
{
    Task<AuthResultDto?> LoginAsync(string email, string senha);
    Task<AuthResultDto?> RefreshAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
