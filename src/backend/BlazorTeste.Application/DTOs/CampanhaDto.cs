namespace BlazorTeste.Application.DTOs;

public class CampanhaDto
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Assunto { get; set; } = "";
    public string Destinatarios { get; set; } = "";
    public int TotalDestinatarios { get; set; }
    public DateTime? DataEnvio { get; set; }
    public string Status { get; set; } = "";
    public string Criador { get; set; } = "";
}
