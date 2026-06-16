namespace BlazorTeste.Models;

public enum StatusCobranca { Pendente, Pago, Vencido, Cancelado }
public enum TipoCobranca { Sindical, Confederativa, Associativa, Negocial }

public class Cobranca
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public TipoCobranca Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusCobranca Status { get; set; }
    public string LinhaDigitavel { get; set; } = "";
    public int AnoReferencia { get; set; }
}
