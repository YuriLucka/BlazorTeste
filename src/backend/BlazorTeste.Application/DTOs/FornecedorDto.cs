namespace BlazorTeste.Application.DTOs;

public class FornecedorDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
}
