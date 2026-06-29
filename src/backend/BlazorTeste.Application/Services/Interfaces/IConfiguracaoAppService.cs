using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IConfiguracaoAppService
{
    Task<ConfiguracaoBancoDto?> GetBancoAsync(int entidadeId);
}
