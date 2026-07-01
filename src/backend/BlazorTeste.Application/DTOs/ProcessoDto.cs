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
