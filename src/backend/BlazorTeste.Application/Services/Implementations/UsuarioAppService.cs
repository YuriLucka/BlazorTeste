using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorTeste.Application.Services.Implementations;

public class UsuarioAppService(UserManager<ApplicationUser> userManager) : IUsuarioAppService
{
    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var items = await userManager.Users.ToListAsync();
        return items.Select(Map);
    }

    public async Task<UsuarioDto?> GetByEmailAsync(string email)
    {
        var u = await userManager.FindByEmailAsync(email);
        return u is null ? null : Map(u);
    }

    private static UsuarioDto Map(ApplicationUser u) => new()
    {
        Id = u.Id,
        Nome = u.Nome,
        Email = u.Email!,
        UltimoAcesso = u.UltimoAcesso,
        Permissoes = u.Permissoes.Select(p => new PermissaoEntidadeDto
        {
            EntidadeId = p.EntidadeId,
            NomeEntidade = p.NomeEntidade,
            Modulos = p.Modulos
        }).ToList()
    };
}
