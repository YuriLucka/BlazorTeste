using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class NegociacaoAppService : INegociacaoAppService
{
    private readonly INegociacaoRepository _repo;

    public NegociacaoAppService(INegociacaoRepository repo) => _repo = repo;

    public async Task<IEnumerable<NegociacaoDto>> GetAllAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAsync()
            : await _repo.GetByEntidadeAsync(entidadeId);
        return items.Select(n => new NegociacaoDto
        {
            Id = n.Id,
            EntidadeId = n.EntidadeId,
            ContribuinteId = n.ContribuinteId,
            RazaoSocialContribuinte = n.RazaoSocialContribuinte,
            DataAbertura = n.DataAbertura,
            DataConclusao = n.DataConclusao,
            DebitoOriginal = n.DebitoOriginal,
            ValorNegociado = n.ValorNegociado,
            Desconto = n.Desconto,
            NumeroParcelas = n.NumeroParcelas,
            Status = n.Status.ToString(),
            Observacoes = n.Observacoes
        });
    }
}
