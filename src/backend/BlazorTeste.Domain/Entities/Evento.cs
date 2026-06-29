using BlazorTeste.Domain.Enums;

namespace BlazorTeste.Domain.Entities;

public class Evento
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
    public StatusEvento Status { get; set; }
    public List<InscricaoEvento> Inscricoes { get; set; } = new();
}

public class InscricaoEvento
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public string NomeParticipante { get; set; } = "";
    public string Email { get; set; } = "";
    public string Empresa { get; set; } = "";
    public string Cargo { get; set; } = "";
    public DateTime DataInscricao { get; set; }
    public bool Pago { get; set; }
    public bool Presente { get; set; }
    public string TipoHospedagem { get; set; } = "Sem hospedagem";
}
