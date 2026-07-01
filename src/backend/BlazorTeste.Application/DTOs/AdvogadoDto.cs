namespace BlazorTeste.Application.DTOs;

public class AdvogadoDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Oab { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
    public int ProcessosAtivos { get; set; }
}
