namespace BlazorTeste.Application.DTOs;

public class ContribuinteDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string RazaoSocial { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Cnae { get; set; } = "";
    public decimal CapitalSocial { get; set; }
    public int NumeroFuncionarios { get; set; }
    public string RegimeTributario { get; set; } = "";
    public DateTime DataCadastro { get; set; }
    public DateTime DataAbertura { get; set; }
    public string Situacao { get; set; } = "Ativo";
}
