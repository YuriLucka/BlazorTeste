namespace BlazorTeste.Application.DTOs;

public class EntidadeDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Sigla { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string CidadeSede { get; set; } = "";
    public int TotalContribuintes { get; set; }
}
