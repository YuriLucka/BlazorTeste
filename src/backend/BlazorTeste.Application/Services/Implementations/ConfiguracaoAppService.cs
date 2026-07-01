using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class ConfiguracaoAppService(IConfiguracaoRepository repo) : IConfiguracaoAppService
{
    public async Task<ConfiguracaoBancoDto?> GetBancoAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        if (config is null) return null;
        return new ConfiguracaoBancoDto
        {
            Banco = config.Banco.Banco,
            Agencia = config.Banco.Agencia,
            Conta = config.Banco.Conta,
            Cedente = config.Banco.Cedente,
            CodigoCedente = config.Banco.CodigoCedente,
            Carteira = config.Banco.Carteira
        };
    }

    public async Task<ConfiguracaoGeral?> GetGeralAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        return config?.Geral;
    }

    public async Task<ConfiguracaoCobranca?> GetCobrancaAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        return config?.Cobranca;
    }

    public async Task<ConfiguracaoEmail?> GetEmailAsync(int entidadeId)
    {
        var config = await repo.GetByEntidadeAsync(entidadeId);
        return config?.Email;
    }
}
