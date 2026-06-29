namespace BlazorTeste.Application.DTOs;

public class EventoDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string Local { get; set; } = "";
    public string Cidade { get; set; } = "";
    public string Estado { get; set; } = "SP";
    public int MaxParticipantes { get; set; }
    public int TotalInscritos { get; set; }
    public decimal TaxaInscricao { get; set; }
    public string Status { get; set; } = "";
}
