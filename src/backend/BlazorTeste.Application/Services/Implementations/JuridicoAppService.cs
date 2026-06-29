using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class JuridicoAppService : IJuridicoAppService
{
    private readonly IJuridicoRepository _repo;

    public JuridicoAppService(IJuridicoRepository repo) => _repo = repo;

    public async Task<IEnumerable<ProcessoDto>> GetProcessosAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAsync()
            : await _repo.GetByEntidadeAsync(entidadeId);
        return items.Select(p => new ProcessoDto
        {
            Id = p.Id,
            EntidadeId = p.EntidadeId,
            Numero = p.Numero,
            Tipo = p.Tipo,
            Vara = p.Vara,
            Tribunal = p.Tribunal,
            Situacao = p.Situacao,
            AdvogadoId = p.AdvogadoId,
            NomeAdvogado = p.NomeAdvogado,
            DataAbertura = p.DataAbertura,
            Descricao = p.Descricao
        });
    }

    public async Task<IEnumerable<AdvogadoDto>> GetAdvogadosAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAdvogadosAsync()
            : await _repo.GetAdvogadosAsync(entidadeId);
        return items.Select(a => new AdvogadoDto
        {
            Id = a.Id,
            EntidadeId = a.EntidadeId,
            Nome = a.Nome,
            Oab = a.Oab,
            Email = a.Email,
            Telefone = a.Telefone,
            ProcessosAtivos = a.ProcessosAtivos
        });
    }

    public async Task<IEnumerable<AudienciaDto>> GetAudienciasAsync()
    {
        var items = await _repo.GetAllAudienciasAsync();
        return items.Select(a => new AudienciaDto
        {
            Id = a.Id,
            ProcessoId = a.ProcessoId,
            NumeroProcesso = a.NumeroProcesso,
            AdvogadoId = a.AdvogadoId,
            NomeAdvogado = a.NomeAdvogado,
            DataHora = a.DataHora,
            Tipo = a.Tipo,
            Local = a.Local,
            Situacao = a.Situacao
        });
    }
}
