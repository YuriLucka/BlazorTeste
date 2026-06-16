using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class EntidadeService
{
    private readonly List<Entidade> _entidades = new()
    {
        new() { Id = 1, Nome = "Sindicato de Hotéis e Meios de Hospedagem", Sigla = "SINDHOSP", Cnpj = "12.345.678/0001-90", CidadeSede = "São Paulo", TotalContribuintes = 1243, Cnaes = new() { "5510-8/01", "5590-6/01", "5590-6/02" }, Cidades = new() { "São Paulo", "Guarulhos", "Osasco" } },
        new() { Id = 2, Nome = "Sindicato de Bares e Restaurantes", Sigla = "SINDBAR", Cnpj = "23.456.789/0001-01", CidadeSede = "São Paulo", TotalContribuintes = 8921, Cnaes = new() { "5611-2/01", "5611-2/03", "5612-1/00" }, Cidades = new() { "São Paulo", "São Bernardo do Campo", "Santo André" } },
        new() { Id = 3, Nome = "Sindicato de Empresas de Turismo", Sigla = "SINDETUR", Cnpj = "34.567.890/0001-12", CidadeSede = "São Paulo", TotalContribuintes = 562, Cnaes = new() { "7911-2/00", "7912-1/00", "7990-2/00" }, Cidades = new() { "São Paulo", "Campinas", "Sorocaba" } },
        new() { Id = 4, Nome = "Sindicato de Promotores de Eventos", Sigla = "SINDEVEN", Cnpj = "45.678.901/0001-23", CidadeSede = "São Paulo", TotalContribuintes = 387, Cnaes = new() { "8230-0/01", "8230-0/02" }, Cidades = new() { "São Paulo", "Barueri", "Cotia" } },
        new() { Id = 5, Nome = "Federação Estadual de Turismo e Hospitalidade", Sigla = "FETURH", Cnpj = "56.789.012/0001-34", CidadeSede = "São Paulo", TotalContribuintes = 4, Cnaes = new(), Cidades = new() { "São Paulo" } }
    };

    public List<Entidade> GetAll() => _entidades;
    public Entidade? GetById(int id) => _entidades.FirstOrDefault(e => e.Id == id);
}
