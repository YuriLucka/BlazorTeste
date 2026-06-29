using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Enums;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class CobrancaAppService : ICobrancaAppService
{
    private readonly ICobrancaRepository _repo;

    public CobrancaAppService(ICobrancaRepository repo) => _repo = repo;

    private static CobrancaDto Map(Domain.Entities.Cobranca c) => new()
    {
        Id = c.Id,
        EntidadeId = c.EntidadeId,
        ContribuinteId = c.ContribuinteId,
        RazaoSocialContribuinte = c.RazaoSocialContribuinte,
        Tipo = c.Tipo.ToString(),
        Valor = c.Valor,
        Multa = c.Multa,
        Juros = c.Juros,
        Vencimento = c.Vencimento,
        DataPagamento = c.DataPagamento,
        Status = c.Status.ToString(),
        LinhaDigitavel = c.LinhaDigitavel,
        AnoReferencia = c.AnoReferencia
    };

    public async Task<IEnumerable<CobrancaDto>> GetAllAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAsync()
            : await _repo.GetByEntidadeAsync(entidadeId);
        return items.Select(Map);
    }

    public async Task<IEnumerable<CobrancaDto>> GetByStatusAsync(int entidadeId, string status)
    {
        if (!Enum.TryParse<StatusCobranca>(status, ignoreCase: true, out var statusEnum))
            return Enumerable.Empty<CobrancaDto>();

        var items = await _repo.GetByStatusAsync(statusEnum);
        return (entidadeId == 0 ? items : items.Where(c => c.EntidadeId == entidadeId)).Select(Map);
    }

    public async Task<IEnumerable<CobrancaDto>> GetVencidasAsync(int entidadeId)
    {
        var items = await _repo.GetVencidasAsync();
        return (entidadeId == 0 ? items : items.Where(c => c.EntidadeId == entidadeId)).Select(Map);
    }
}
