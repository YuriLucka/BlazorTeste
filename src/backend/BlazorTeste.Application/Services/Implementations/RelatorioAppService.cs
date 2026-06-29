using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class RelatorioAppService : IRelatorioAppService
{
    private readonly IRepository<Domain.Entities.Relatorio> _repo;

    public RelatorioAppService(IRepository<Domain.Entities.Relatorio> repo) => _repo = repo;

    public async Task<IEnumerable<RelatorioDto>> GetAllAsync(int entidadeId)
    {
        // Relatorio is a catalog entity, not scoped per entidade
        var items = await _repo.GetAllAsync();
        return items.Select(r => new RelatorioDto
        {
            Id = r.Id,
            Nome = r.Nome,
            Descricao = r.Descricao,
            Categoria = r.Categoria.ToString(),
            Formato = r.Formato.ToString(),
            Icone = r.Icone
        });
    }
}
