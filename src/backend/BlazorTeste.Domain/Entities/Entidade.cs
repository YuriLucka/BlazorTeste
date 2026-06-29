namespace BlazorTeste.Domain.Entities;

public class Entidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Sigla { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string CidadeSede { get; set; } = "";
    public int TotalContribuintes { get; set; }
    public List<string> Cnaes { get; set; } = new();
    public List<string> Cidades { get; set; } = new();
}
