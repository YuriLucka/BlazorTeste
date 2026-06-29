using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class FinanceiroAppService : IFinanceiroAppService
{
    private readonly IFinanceiroRepository _financeiroRepo;
    private readonly IContribuinteRepository _contribuinteRepo;
    private readonly ICobrancaRepository _cobrancaRepo;
    private readonly IJuridicoRepository _juridicoRepo;

    public FinanceiroAppService(
        IFinanceiroRepository financeiroRepo,
        IContribuinteRepository contribuinteRepo,
        ICobrancaRepository cobrancaRepo,
        IJuridicoRepository juridicoRepo)
    {
        _financeiroRepo = financeiroRepo;
        _contribuinteRepo = contribuinteRepo;
        _cobrancaRepo = cobrancaRepo;
        _juridicoRepo = juridicoRepo;
    }

    public async Task<IEnumerable<LancamentoFinanceiroDto>> GetLancamentosAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _financeiroRepo.GetAllAsync()
            : await _financeiroRepo.GetByEntidadeAsync(entidadeId);
        return items.Select(l => new LancamentoFinanceiroDto
        {
            Id = l.Id,
            EntidadeId = l.EntidadeId,
            Data = l.Data,
            Categoria = l.Categoria,
            Descricao = l.Descricao,
            FornecedorId = l.FornecedorId,
            NomeFornecedor = l.NomeFornecedor,
            Valor = l.Valor,
            Tipo = l.Tipo.ToString(),
            ContaBancaria = l.ContaBancaria,
            Realizado = l.Realizado
        });
    }

    public async Task<IEnumerable<FornecedorDto>> GetFornecedoresAsync()
    {
        var items = await _financeiroRepo.GetAllAsync();
        // Return distinct fornecedores from all lancamentos that have one
        var fornecedores = items
            .Where(l => l.FornecedorId.HasValue)
            .GroupBy(l => l.FornecedorId)
            .Select(g => g.First())
            .Select(l => new FornecedorDto
            {
                Id = l.FornecedorId!.Value,
                EntidadeId = l.EntidadeId,
                Nome = l.NomeFornecedor
            });
        return fornecedores;
    }

    public async Task<DashboardDto> GetDashboardAsync(int entidadeId)
    {
        var contribuintes = await _contribuinteRepo.GetByEntidadeAsync(entidadeId);
        var cobrancas = await _cobrancaRepo.GetByEntidadeAsync(entidadeId);
        var processos = await _juridicoRepo.GetByEntidadeAsync(entidadeId);
        var lancamentos = await _financeiroRepo.GetByEntidadeAsync(entidadeId);

        var totalReceita = lancamentos
            .Where(l => l.Tipo == Domain.Enums.TipoLancamento.Entrada && l.Realizado)
            .Sum(l => l.Valor);

        return new DashboardDto
        {
            TotalContribuintes = contribuintes.Count,
            TotalCobrancas = cobrancas.Count,
            TotalProcessos = processos.Count,
            TotalReceita = totalReceita
        };
    }
}
