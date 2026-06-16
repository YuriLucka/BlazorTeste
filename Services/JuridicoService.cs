using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class JuridicoService
{
    private readonly List<Advogado> _advogados;
    private readonly List<Processo> _processos;
    private readonly List<Audiencia> _audiencias;

    public JuridicoService()
    {
        _advogados = new List<Advogado>
        {
            new() { Id = 1, EntidadeId = 1, Nome = "Dr. Carlos Eduardo Mendes", Oab = "OAB/SP 123.456", Email = "carlos.mendes@jurmendes.adv.br", Telefone = "(11) 3456-7890", ProcessosAtivos = 5 },
            new() { Id = 2, EntidadeId = 1, Nome = "Dra. Fernanda Lima Costa", Oab = "OAB/SP 234.567", Email = "fernanda.costa@fclima.adv.br", Telefone = "(11) 4567-8901", ProcessosAtivos = 3 },
            new() { Id = 3, EntidadeId = 2, Nome = "Dr. Ricardo Alves Pinheiro", Oab = "OAB/SP 345.678", Email = "r.pinheiro@rapinheiro.adv.br", Telefone = "(11) 5678-9012", ProcessosAtivos = 7 },
            new() { Id = 4, EntidadeId = 2, Nome = "Dra. Amanda Souza Ferreira", Oab = "OAB/SP 456.789", Email = "amanda.ferreira@asferreira.adv.br", Telefone = "(11) 6789-0123", ProcessosAtivos = 4 },
            new() { Id = 5, EntidadeId = 3, Nome = "Dr. Marcelo Torres Braga", Oab = "OAB/SP 567.890", Email = "marcelo.braga@mtbraga.adv.br", Telefone = "(11) 7890-1234", ProcessosAtivos = 2 },
            new() { Id = 6, EntidadeId = 3, Nome = "Dra. Patrícia Gomes Vieira", Oab = "OAB/SP 678.901", Email = "patricia.vieira@pgvieira.adv.br", Telefone = "(11) 8901-2345", ProcessosAtivos = 6 },
            new() { Id = 7, EntidadeId = 4, Nome = "Dr. Roberto Nascimento Silva", Oab = "OAB/SP 789.012", Email = "roberto.silva@rnsilva.adv.br", Telefone = "(11) 9012-3456", ProcessosAtivos = 1 },
            new() { Id = 8, EntidadeId = 1, Nome = "Dra. Juliana Castro Moreira", Oab = "OAB/SP 890.123", Email = "juliana.moreira@jcmoreira.adv.br", Telefone = "(11) 0123-4567", ProcessosAtivos = 3 }
        };

        var rng = new Random(42);
        var tiposP = new[] { "Reclamação trabalhista", "Ação de cobrança", "Mandado de segurança", "Ação civil pública", "Execução fiscal" };
        var varas = new[] { "1ª Vara do Trabalho", "2ª Vara do Trabalho", "3ª Vara Cível", "4ª Vara Federal" };
        var tribunais = new[] { "TRT 2ª Região", "TJSP", "TRF 3ª Região" };
        var situacoes = new[] { "Em andamento", "Em andamento", "Em andamento", "Suspenso", "Encerrado" };

        _processos = Enumerable.Range(1, 15).Select(i =>
        {
            var adv = _advogados[rng.Next(_advogados.Count)];
            return new Processo
            {
                Id = i,
                EntidadeId = adv.EntidadeId,
                Numero = $"{rng.Next(1000, 9999)}-{rng.Next(10, 99)}.{DateTime.Today.Year}.5.02.{rng.Next(1000, 9999)}",
                Tipo = tiposP[rng.Next(tiposP.Length)],
                Vara = varas[rng.Next(varas.Length)],
                Tribunal = tribunais[rng.Next(tribunais.Length)],
                Situacao = situacoes[rng.Next(situacoes.Length)],
                AdvogadoId = adv.Id,
                NomeAdvogado = adv.Nome,
                DataAbertura = DateTime.Today.AddDays(-rng.Next(30, 1825)),
                Descricao = "Processo referente a ação judicial conforme documentação em arquivo."
            };
        }).ToList();

        var tiposA = new[] { "Inicial", "Instrução", "Julgamento", "Conciliação", "Perícia" };
        var locais = new[] { "Foro Central da Barra Funda", "Foro Regional da Lapa", "TRT 2ª Região", "TJSP — Pátio do Colégio" };
        var sitAud = new[] { "Agendada", "Agendada", "Realizada", "Cancelada" };

        _audiencias = Enumerable.Range(1, 20).Select(i =>
        {
            var processo = _processos[rng.Next(_processos.Count)];
            var adv = _advogados.First(a => a.Id == processo.AdvogadoId);
            return new Audiencia
            {
                Id = i,
                ProcessoId = processo.Id,
                NumeroProcesso = processo.Numero,
                AdvogadoId = adv.Id,
                NomeAdvogado = adv.Nome,
                DataHora = DateTime.Today.AddDays(rng.Next(-30, 60)).Date.AddHours(rng.Next(8, 17)),
                Tipo = tiposA[rng.Next(tiposA.Length)],
                Local = locais[rng.Next(locais.Length)],
                Situacao = sitAud[rng.Next(sitAud.Length)]
            };
        }).ToList();
    }

    public List<Advogado> GetAdvogados() => _advogados;
    public List<Processo> GetProcessos() => _processos;
    public List<Audiencia> GetAudiencias() => _audiencias;
}
