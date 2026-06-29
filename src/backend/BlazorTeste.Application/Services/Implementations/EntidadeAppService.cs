using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class EntidadeAppService : IEntidadeAppService
{
    private readonly IEntidadeRepository _repo;

    public EntidadeAppService(IEntidadeRepository repo) => _repo = repo;

    private static EntidadeDto Map(Domain.Entities.Entidade e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        Sigla = e.Sigla,
        Cnpj = e.Cnpj,
        CidadeSede = e.CidadeSede,
        TotalContribuintes = e.TotalContribuintes
    };

    public async Task<IEnumerable<EntidadeDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(Map);
    }

    public async Task<EntidadeDto?> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? null : Map(e);
    }
}
