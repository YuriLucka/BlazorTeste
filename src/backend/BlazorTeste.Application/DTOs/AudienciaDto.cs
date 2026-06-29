namespace BlazorTeste.Application.DTOs;

public class AudienciaDto
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string NumeroProcesso { get; set; } = "";
    public int AdvogadoId { get; set; }
    public string NomeAdvogado { get; set; } = "";
    public DateTime DataHora { get; set; }
    public string Tipo { get; set; } = "";
    public string Local { get; set; } = "";
    public string Situacao { get; set; } = "";
}
