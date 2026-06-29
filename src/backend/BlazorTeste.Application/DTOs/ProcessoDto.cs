namespace BlazorTeste.Application.DTOs;

public class ProcessoDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Numero { get; set; } = "";
    public string Tipo { get; set; } = "";
    public string Vara { get; set; } = "";
    public string Tribunal { get; set; } = "";
    public string Situacao { get; set; } = "";
    public int AdvogadoId { get; set; }
    public string NomeAdvogado { get; set; } = "";
    public DateTime DataAbertura { get; set; }
    public string Descricao { get; set; } = "";
}

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
