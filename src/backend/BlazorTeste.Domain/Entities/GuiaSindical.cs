using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

public class GuiaSindical
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public int AnoReferencia { get; set; }
    public decimal ValorBase { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public decimal ValorTotal => ValorBase + Multa + Juros;
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusGuia Status { get; set; }
    public string LinhaDigitavel { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
    public DateTime DataEmissao { get; set; }
}
