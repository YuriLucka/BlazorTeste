namespace BlazorTeste.Application.DTOs;

public class GerarRelatorioRequest
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Contribuinte { get; set; }
    public string? Status { get; set; }
    public string Formato { get; set; } = "PDF";
}
