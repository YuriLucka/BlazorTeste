using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IUsuarioAppService
{
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<UsuarioDto?> GetByEmailAsync(string email);
}
