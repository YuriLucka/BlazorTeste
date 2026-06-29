using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

public class RegistroBaixa
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int CobrancaId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public string TipoCobranca { get; set; } = "";
    public decimal ValorOriginal { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime DataPagamento { get; set; }
    public DateTime DataProcessamento { get; set; }
    public TipoBaixa TipoBaixa { get; set; }
    public string OperadorResponsavel { get; set; } = "";
    public string Observacoes { get; set; } = "";
}
