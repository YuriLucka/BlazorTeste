using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class UsuarioAppService(IUsuarioRepository repo) : IUsuarioAppService
{
    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var items = await repo.GetAllAsync();
        return items.Select(Map);
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var u = await repo.GetByEmailAsync(email);
        return u is null ? null : Map(u);
    }

    private static UsuarioDto Map(Usuario u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email,
        UltimoAcesso = u.UltimoAcesso,
        Permissoes = u.Permissoes
    };
}
