using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class EventoAppService : IEventoAppService
{
    private readonly IEventoRepository _repo;

    public EventoAppService(IEventoRepository repo) => _repo = repo;

    public async Task<IEnumerable<EventoDto>> GetAllAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAsync()
            : await _repo.GetByEntidadeAsync(entidadeId);
        return items.Select(e => new EventoDto
        {
            Id = e.Id,
            EntidadeId = e.EntidadeId,
            Nome = e.Nome,
            Descricao = e.Descricao,
            DataInicio = e.DataInicio,
            DataFim = e.DataFim,
            Local = e.Local,
            Cidade = e.Cidade,
            Estado = e.Estado,
            MaxParticipantes = e.MaxParticipantes,
            TotalInscritos = e.TotalInscritos,
            TaxaInscricao = e.TaxaInscricao,
            Status = e.Status.ToString()
        });
    }
}
