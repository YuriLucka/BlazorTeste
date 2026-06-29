using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

public class Negociacao
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
    public StatusNegociacao Status { get; set; }
    public string Observacoes { get; set; } = "";
    public List<ParcelaNegociacao> Parcelas { get; set; } = new();
    public List<CobrancaOriginalNeg> CobrancasOriginais { get; set; } = new();
}

public class ParcelaNegociacao
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public decimal Valor { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool Pago { get; set; }
    public string LinhaDigitavel { get; set; } = "";
}

public class CobrancaOriginalNeg
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "";
    public int AnoReferencia { get; set; }
    public decimal ValorOriginal { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
}
