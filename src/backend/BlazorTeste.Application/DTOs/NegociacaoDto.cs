namespace BlazorTeste.Application.DTOs;

public class NegociacaoDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public DateTime DataAbertura { get; set; }
    public DateTime? DataConclusao { get; set; }
    public decimal DebitoOriginal { get; set; }
    public decimal ValorNegociado { get; set; }
    public decimal Desconto { get; set; }
    public int NumeroParcelas { get; set; }
    public string Status { get; set; } = "";
    public string Observacoes { get; set; } = "";
}
