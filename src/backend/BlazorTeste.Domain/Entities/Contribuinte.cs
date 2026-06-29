namespace BlazorTeste.Domain.Entities;

public class Contribuinte
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string RazaoSocial { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Cnae { get; set; } = "";
    public decimal CapitalSocial { get; set; }
    public int NumeroFuncionarios { get; set; }
    public string RegimeTributario { get; set; } = "";
    public DateTime DataCadastro { get; set; }
    public DateTime DataAbertura { get; set; }
    public string Situacao { get; set; } = "Ativo";
    public List<Endereco> Enderecos { get; set; } = new();
    public List<Contato> Contatos { get; set; } = new();
    public List<Socio> Socios { get; set; } = new();
    public List<HistoricoMensal> Historico { get; set; } = new();
}

public class Endereco
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "Estabelecimento";
    public string Logradouro { get; set; } = "";
    public string Numero { get; set; } = "";
    public string Complemento { get; set; } = "";
    public string Bairro { get; set; } = "";
    public string Cidade { get; set; } = "";
    public string Estado { get; set; } = "SP";
    public string Cep { get; set; } = "";
}

public class Contato
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "Email";
    public string Valor { get; set; } = "";
    public string Descricao { get; set; } = "";
}

public class Socio
{
    public int Id { get; set; }
    public int ContribuinteId { get; set; }
    public string Matricula { get; set; } = "";
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public decimal ValorMensalidade { get; set; }
}

public class HistoricoMensal
{
    public int Id { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
    public decimal CapitalSocial { get; set; }
    public int NumeroFuncionarios { get; set; }
}
