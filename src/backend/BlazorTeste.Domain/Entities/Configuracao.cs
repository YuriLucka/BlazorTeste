namespace BlazorTeste.Domain.Entities;

public class ConfiguracaoGeral
{
    public int EntidadeId { get; set; }
    public string ResponsavelTecnico { get; set; } = "";
    public string EmailContato { get; set; } = "";
    public string Telefone { get; set; } = "";
    public string Endereco { get; set; } = "";
}

public class ConfiguracaoCobranca
{
    public int EntidadeId { get; set; }
    public decimal PercentualMulta { get; set; } = 2m;
    public decimal PercentualJurosDia { get; set; } = 0.033m;
    public int DiasCarencia { get; set; } = 5;
    public bool EmissaoAutomatica { get; set; }
    public int DiaVencimento { get; set; } = 30;
    public List<FaixaCobranca> Faixas { get; set; } = new();
}

public class FaixaCobranca
{
    public int Id { get; set; }
    public string Descricao { get; set; } = "";
    public decimal CapitalSocialMin { get; set; }
    public decimal CapitalSocialMax { get; set; }
    public decimal ValorFixo { get; set; }
    public decimal PercentualCapital { get; set; }
}

public class ConfiguracaoEmail
{
    public string ServidorSmtp { get; set; } = "";
    public int Porta { get; set; } = 587;
    public bool UsarSsl { get; set; } = true;
    public string EmailRemetente { get; set; } = "";
    public string NomeRemetente { get; set; } = "";
    public bool Ativo { get; set; }
}

public class ConfiguracaoBanco
{
    public string Banco { get; set; } = "";
    public string Agencia { get; set; } = "";
    public string Conta { get; set; } = "";
    public string Cedente { get; set; } = "";
    public string CodigoCedente { get; set; } = "";
    public string Carteira { get; set; } = "";
}

public class ConfiguracaoEntidade
{
    public int EntidadeId { get; set; }
    public ConfiguracaoGeral Geral { get; set; } = new();
    public ConfiguracaoCobranca Cobranca { get; set; } = new();
    public ConfiguracaoEmail Email { get; set; } = new();
    public ConfiguracaoBanco Banco { get; set; } = new();
}
