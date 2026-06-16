namespace BlazorTeste.Models;

public enum StatusCampanha { Rascunho, Agendada, Enviada, Erro }

public class Campanha
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Assunto { get; set; } = "";
    public string Destinatarios { get; set; } = "";
    public int TotalDestinatarios { get; set; }
    public DateTime? DataEnvio { get; set; }
    public StatusCampanha Status { get; set; }
    public string Criador { get; set; } = "";
}
