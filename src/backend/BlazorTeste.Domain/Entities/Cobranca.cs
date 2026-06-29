using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

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

    public void Pagar(DateTime dataPagamento)
    {
        Status = StatusCobranca.Pago;
        DataPagamento = dataPagamento;
    }

    public void Vencer()
    {
        if (Status == StatusCobranca.Pendente)
            Status = StatusCobranca.Vencido;
    }
}
