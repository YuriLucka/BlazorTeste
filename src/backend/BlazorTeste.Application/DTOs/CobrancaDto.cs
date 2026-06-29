namespace BlazorTeste.Application.DTOs;

public class CobrancaDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public string Tipo { get; set; } = "";
    public decimal Valor { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public string Status { get; set; } = "";
    public string LinhaDigitavel { get; set; } = "";
    public int AnoReferencia { get; set; }
}
