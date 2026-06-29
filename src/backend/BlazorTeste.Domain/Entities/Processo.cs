namespace BlazorTeste.Domain.Entities;

public class Processo
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

    public void Encerrar()
    {
        Situacao = "Encerrado";
    }

    public void Suspender()
    {
        Situacao = "Suspenso";
    }
}

public class Advogado
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Oab { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
    public int ProcessosAtivos { get; set; }
}

public class Audiencia
{
    public int Id { get; set; }
    public int ProcessoId { get; set; }
    public string NumeroProcesso { get; set; } = "";
    public int AdvogadoId { get; set; }
    public string NomeAdvogado { get; set; } = "";
    public DateTime DataHora { get; set; }
    public string Tipo { get; set; } = "";
    public string Local { get; set; } = "";
    public string Situacao { get; set; } = "Agendada";
}
