using BlazorTeste.Application.DTOs;

namespace BlazorTeste.Application.Services.Interfaces;

public interface IBaixaCobrancaAppService
{
    Task<IEnumerable<CobrancaDto>> GetVencidasAsync(int entidadeId);
    Task<BaixaCobrancaDto> BaixarAsync(int cobrancaId, string tipo);
    Task<ProcessarArquivoDto> ProcessarArquivoAsync(int entidadeId);
}
