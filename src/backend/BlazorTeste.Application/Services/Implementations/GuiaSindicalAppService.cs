using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class GuiaSindicalAppService : IGuiaSindicalAppService
{
    private readonly IGuiaSindicalRepository _repo;

    public GuiaSindicalAppService(IGuiaSindicalRepository repo) => _repo = repo;

    public async Task<IEnumerable<GuiaSindicalDto>> GetAllAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAsync()
            : await _repo.GetByEntidadeAsync(entidadeId);
        return items.Select(g => new GuiaSindicalDto
        {
            Id = g.Id,
            EntidadeId = g.EntidadeId,
            ContribuinteId = g.ContribuinteId,
            RazaoSocialContribuinte = g.RazaoSocialContribuinte,
            Cnpj = g.Cnpj,
            AnoReferencia = g.AnoReferencia,
            ValorBase = g.ValorBase,
            Multa = g.Multa,
            Juros = g.Juros,
            ValorTotal = g.ValorTotal,
            Vencimento = g.Vencimento,
            DataPagamento = g.DataPagamento,
            Status = g.Status.ToString(),
            LinhaDigitavel = g.LinhaDigitavel,
            NumeroDocumento = g.NumeroDocumento,
            DataEmissao = g.DataEmissao
        });
    }
}
