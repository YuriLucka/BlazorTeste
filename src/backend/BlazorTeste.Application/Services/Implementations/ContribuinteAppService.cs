using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Services.Interfaces;
using BlazorTeste.Domain.Interfaces.Repositories;

namespace BlazorTeste.Application.Services.Implementations;

public class ContribuinteAppService : IContribuinteAppService
{
    private readonly IContribuinteRepository _repo;

    public ContribuinteAppService(IContribuinteRepository repo) => _repo = repo;

    public async Task<IEnumerable<ContribuinteDto>> GetAllAsync(int entidadeId)
    {
        var items = entidadeId == 0
            ? await _repo.GetAllAsync()
            : await _repo.GetByEntidadeAsync(entidadeId);
        return items.Select(c => new ContribuinteDto
        {
            Id = c.Id,
            EntidadeId = c.EntidadeId,
            RazaoSocial = c.RazaoSocial,
            Cnpj = c.Cnpj,
            Cnae = c.Cnae,
            CapitalSocial = c.CapitalSocial,
            NumeroFuncionarios = c.NumeroFuncionarios,
            RegimeTributario = c.RegimeTributario,
            DataCadastro = c.DataCadastro,
            DataAbertura = c.DataAbertura,
            Situacao = c.Situacao
        });
    }

    public async Task<ContribuinteDto?> GetByIdAsync(int id)
    {
        var c = await _repo.GetByIdAsync(id);
        if (c == null) return null;
        return new ContribuinteDto
        {
            Id = c.Id,
            EntidadeId = c.EntidadeId,
            RazaoSocial = c.RazaoSocial,
            Cnpj = c.Cnpj,
            Cnae = c.Cnae,
            CapitalSocial = c.CapitalSocial,
            NumeroFuncionarios = c.NumeroFuncionarios,
            RegimeTributario = c.RegimeTributario,
            DataCadastro = c.DataCadastro,
            DataAbertura = c.DataAbertura,
            Situacao = c.Situacao
        };
    }
}
