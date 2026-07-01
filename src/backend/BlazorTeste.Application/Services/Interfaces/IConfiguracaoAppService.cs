using BlazorTeste.Application.DTOs;
using BlazorTeste.Domain.Entities;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IConfiguracaoAppService
{
    Task<ConfiguracaoBancoDto?> GetBancoAsync(int entidadeId);
    Task<ConfiguracaoGeral?> GetGeralAsync(int entidadeId);
    Task<ConfiguracaoCobranca?> GetCobrancaAsync(int entidadeId);
    Task<ConfiguracaoEmail?> GetEmailAsync(int entidadeId);
}
