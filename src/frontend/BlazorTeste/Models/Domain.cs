namespace BlazorTeste.Models;

// ─── Entidades ────────────────────────────────────────────────────────────────

public class Entidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Sigla { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string CidadeSede { get; set; } = "";
    public int TotalContribuintes { get; set; }
    public List<string> Cnaes { get; set; } = new();
    public List<string> Cidades { get; set; } = new();
}

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

// ─── Cobrança ─────────────────────────────────────────────────────────────────

public enum TipoCobranca { Sindical, Confederativa, Associativa, Negocial }
public enum StatusCobranca { Pendente, Pago, Vencido, Cancelado }

public class Cobranca
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public TipoCobranca Tipo { get; set; }
    public decimal Valor { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusCobranca Status { get; set; }
    public string LinhaDigitavel { get; set; } = "";
    public int AnoReferencia { get; set; }
}

// ─── Baixa de Cobrança ────────────────────────────────────────────────────────

public enum TipoBaixa { Manual, ArquivoRetorno, Automatica }

public class RegistroBaixa
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int CobrancaId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public string TipoCobranca { get; set; } = "";
    public decimal ValorOriginal { get; set; }
    public decimal ValorPago { get; set; }
    public DateTime DataPagamento { get; set; }
    public DateTime DataProcessamento { get; set; }
    public TipoBaixa TipoBaixa { get; set; }
    public string OperadorResponsavel { get; set; } = "";
    public string Observacoes { get; set; } = "";
}

// ─── Jurídico ─────────────────────────────────────────────────────────────────

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

// ─── Financeiro ───────────────────────────────────────────────────────────────

public enum TipoLancamento { Entrada, Saida }

public class LancamentoFinanceiro
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public DateTime Data { get; set; }
    public string Categoria { get; set; } = "";
    public string Descricao { get; set; } = "";
    public int? FornecedorId { get; set; }
    public string NomeFornecedor { get; set; } = "";
    public decimal Valor { get; set; }
    public TipoLancamento Tipo { get; set; }
    public string ContaBancaria { get; set; } = "";
    public bool Realizado { get; set; }
}

public class Fornecedor
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public string Nome { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Email { get; set; } = "";
    public string Telefone { get; set; } = "";
}

// ─── Mailing ──────────────────────────────────────────────────────────────────

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

// ─── Guia Sindical ────────────────────────────────────────────────────────────

public enum StatusGuia { Pendente, Paga, Vencida, Cancelada }

public class GuiaSindical
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public string Cnpj { get; set; } = "";
    public int AnoReferencia { get; set; }
    public decimal ValorBase { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
    public decimal ValorTotal => ValorBase + Multa + Juros;
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusGuia Status { get; set; }
    public string LinhaDigitavel { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
    public DateTime DataEmissao { get; set; }
}

// ─── Negociação ───────────────────────────────────────────────────────────────

public enum StatusNegociacao { EmAndamento, Concluida, Cancelada, Inadimplente }

public class Negociacao
{
    public int Id { get; set; }
    public int EntidadeId { get; set; }
    public int ContribuinteId { get; set; }
    public string RazaoSocialContribuinte { get; set; } = "";
    public DateTime DataAbertura { get; set; }
    public DateTime? DataConclusao { get; set; }
    public decimal DebitoOriginal { get; set; }
    public decimal ValorNegociado { get; set; }
    public decimal Desconto { get; set; }
    public int NumeroParcelas { get; set; }
    public StatusNegociacao Status { get; set; }
    public string Observacoes { get; set; } = "";
    public List<ParcelaNegociacao> Parcelas { get; set; } = new();
    public List<CobrancaOriginalNeg> CobrancasOriginais { get; set; } = new();
}

public class ParcelaNegociacao
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public decimal Valor { get; set; }
    public DateTime Vencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public bool Pago { get; set; }
    public string LinhaDigitavel { get; set; } = "";
}

public class CobrancaOriginalNeg
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "";
    public int AnoReferencia { get; set; }
    public decimal ValorOriginal { get; set; }
    public decimal Multa { get; set; }
    public decimal Juros { get; set; }
}

// ─── Eventos ──────────────────────────────────────────────────────────────────

public enum StatusEvento { Aberto, Fechado, Realizado, Cancelado }

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

// ─── Configurações ────────────────────────────────────────────────────────────

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

// ─── Relatórios ───────────────────────────────────────────────────────────────

public enum CategoriaRelatorio { Cobrancas, Contribuintes, Financeiro, Juridico, Eventos, GuiaSindical }
public enum FormatoRelatorio { PDF, Excel, Ambos }

public class Relatorio
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Descricao { get; set; } = "";
    public CategoriaRelatorio Categoria { get; set; }
    public FormatoRelatorio Formato { get; set; }
    public string Icone { get; set; } = "";
}

// ─── Relatório gerar ──────────────────────────────────────────────────────────

public class GerarRelatorioRequest
{
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Contribuinte { get; set; }
    public string? Status { get; set; }
    public string Formato { get; set; } = "PDF";
}

public class GerarRelatorioResult
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = "";
}

public class ProcessarArquivoResult
{
    public int CobrancasBaixadas { get; set; }
    public int Erros { get; set; }
    public string Mensagem { get; set; } = "";
}

// ─── Usuários ─────────────────────────────────────────────────────────────────

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime UltimoAcesso { get; set; }
    public List<PermissaoEntidade> Permissoes { get; set; } = new();
}

public class PermissaoEntidade
{
    public int EntidadeId { get; set; }
    public string NomeEntidade { get; set; } = "";
    public List<string> Modulos { get; set; } = new();
}
