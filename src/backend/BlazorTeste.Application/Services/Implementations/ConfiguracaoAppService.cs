using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;

namespace BlazorTeste.Application.Services.Implementations;

/// <summary>
/// Placeholder implementation — ConfiguracaoEntidade has no dedicated domain repository.
/// Replace with a proper repository injection when available.
/// </summary>
public class ConfiguracaoAppService : IConfiguracaoAppService
{
    public Task<ConfiguracaoBancoDto?> GetBancoAsync(int entidadeId)
    {
        // TODO: inject a repository or HTTP client once the infrastructure layer exposes one
        return Task.FromResult<ConfiguracaoBancoDto?>(new ConfiguracaoBancoDto());
    }
}
