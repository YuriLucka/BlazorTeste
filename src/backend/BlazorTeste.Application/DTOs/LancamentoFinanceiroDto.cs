namespace BlazorTeste.Application.DTOs;

public class LancamentoFinanceiroDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public DateTime Data { get; set; }
    public string Categoria { get; set; } = "";
    public string Descricao { get; set; } = "";
    public int? FornecedorId { get; set; }
    public string NomeFornecedor { get; set; } = "";
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = "";
    public string ContaBancaria { get; set; } = "";
    public bool Realizado { get; set; }
}
