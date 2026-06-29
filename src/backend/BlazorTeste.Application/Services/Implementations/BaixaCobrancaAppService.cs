using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class BaixaCobrancaAppService : IBaixaCobrancaAppService
{
    private readonly ICobrancaRepository _cobrancaRepo;
    private readonly IBaixaCobrancaRepository _baixaRepo;

    public BaixaCobrancaAppService(
        ICobrancaRepository cobrancaRepo,
        IBaixaCobrancaRepository baixaRepo)
    {
        _cobrancaRepo = cobrancaRepo;
        _baixaRepo = baixaRepo;
    }

    public async Task<IEnumerable<CobrancaDto>> GetVencidasAsync(int entidadeId)
    {
        var items = await _cobrancaRepo.GetVencidasAsync();
        return (entidadeId == 0 ? items : items.Where(c => c.EntidadeId == entidadeId))
            .Select(c => new CobrancaDto
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
            });
    }

    public async Task<BaixaCobrancaDto> BaixarAsync(int cobrancaId, string tipo)
    {
        var cobranca = await _cobrancaRepo.GetByIdAsync(cobrancaId)
            ?? throw new InvalidOperationException($"Cobranca {cobrancaId} not found.");

        if (!Enum.TryParse<TipoBaixa>(tipo, ignoreCase: true, out var tipoBaixa))
            tipoBaixa = TipoBaixa.Manual;

        var agora = DateTime.UtcNow;
        cobranca.Pagar(agora);
        await _cobrancaRepo.UpdateAsync(cobranca);

        var registro = new RegistroBaixa
        {
            EntidadeId = cobranca.EntidadeId,
            CobrancaId = cobranca.Id,
            RazaoSocialContribuinte = cobranca.RazaoSocialContribuinte,
            TipoCobranca = cobranca.Tipo.ToString(),
            ValorOriginal = cobranca.Valor,
            ValorPago = cobranca.Valor + cobranca.Multa + cobranca.Juros,
            DataPagamento = agora,
            DataProcessamento = agora,
            TipoBaixa = tipoBaixa,
            OperadorResponsavel = "Sistema"
        };

        await _baixaRepo.AddAsync(registro);

        return new BaixaCobrancaDto
        {
            Id = registro.Id,
            EntidadeId = registro.EntidadeId,
            CobrancaId = registro.CobrancaId,
            RazaoSocialContribuinte = registro.RazaoSocialContribuinte,
            TipoCobranca = registro.TipoCobranca,
            ValorOriginal = registro.ValorOriginal,
            ValorPago = registro.ValorPago,
            DataPagamento = registro.DataPagamento,
            DataProcessamento = registro.DataProcessamento,
            TipoBaixa = registro.TipoBaixa.ToString(),
            OperadorResponsavel = registro.OperadorResponsavel,
            Observacoes = registro.Observacoes
        };
    }
}
