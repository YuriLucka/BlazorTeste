using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

public class LancamentoFinanceiro
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public DateTime Data { get; set; }
    public string Categoria { get; set; } = "";
    public string Descricao { get; set; } = "";
    public int? FornecedorId { get; set; }
    public string NomeFornecedor { get; set; } = "";
    public decimal Valor { get; set; }
    public TipoLancamento Tipo { get; set; }
    public string ContaBancaria { get; set; } = "";
    public bool Realizado { get; set; }
}

public class Fornecedor
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
}
