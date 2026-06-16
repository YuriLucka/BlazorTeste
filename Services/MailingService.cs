using BlazorTeste.Models;

namespace BlazorTeste.Services;

public class MailingService
{
    private readonly List<Campanha> _campanhas = new()
    {
        new() { Id = 1, EntidadeId = 1, Assunto = "Comunicado — Assembleia Geral Ordinária 2026", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 1243, DataEnvio = DateTime.Today.AddDays(-30), Status = StatusCampanha.Enviada, Criador = "Ana Lima" },
        new() { Id = 2, EntidadeId = 1, Assunto = "Boleto de Contribuição Sindical — Vencimento Abril/2026", Destinatarios = "Contribuintes ativos", TotalDestinatarios = 1180, DataEnvio = DateTime.Today.AddDays(-5), Status = StatusCampanha.Enviada, Criador = "Carlos Silva" },
        new() { Id = 3, EntidadeId = 2, Assunto = "Edital — Eleição Diretoria 2026–2029", Destinatarios = "Sócios ativos", TotalDestinatarios = 420, DataEnvio = DateTime.Today.AddDays(7), Status = StatusCampanha.Agendada, Criador = "Fernanda Costa" },
        new() { Id = 4, EntidadeId = 2, Assunto = "Novo Portal do Contribuinte — Acesse já!", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 8921, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Pedro Martins" },
        new() { Id = 5, EntidadeId = 3, Assunto = "Convenção Coletiva 2026 — Resultado das Negociações", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 562, DataEnvio = DateTime.Today.AddDays(-15), Status = StatusCampanha.Enviada, Criador = "Lucia Fernandes" },
        new() { Id = 6, EntidadeId = 1, Assunto = "Lembrete — Prazo Final Contribuição Confederativa", Destinatarios = "Contribuintes com cobrança pendente", TotalDestinatarios = 287, DataEnvio = DateTime.Today.AddDays(-2), Status = StatusCampanha.Erro, Criador = "Carlos Silva" },
        new() { Id = 7, EntidadeId = 4, Assunto = "Curso de Capacitação — Gestão de Eventos 2026", Destinatarios = "Sócios ativos", TotalDestinatarios = 150, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Beatriz Nunes" },
        new() { Id = 8, EntidadeId = 2, Assunto = "Relatório Mensal — Negociações Coletivas Março/2026", Destinatarios = "Contribuintes sócios", TotalDestinatarios = 320, DataEnvio = DateTime.Today.AddDays(14), Status = StatusCampanha.Agendada, Criador = "Fernanda Costa" },
        new() { Id = 9, EntidadeId = 1, Assunto = "Atualização Cadastral Obrigatória — Prazo até 30/06", Destinatarios = "Todos os contribuintes", TotalDestinatarios = 1243, DataEnvio = DateTime.Today.AddDays(-60), Status = StatusCampanha.Enviada, Criador = "Ana Lima" },
        new() { Id = 10, EntidadeId = 3, Assunto = "Benefícios para Sócios — Novidades 2026", Destinatarios = "Contribuintes", TotalDestinatarios = 562, DataEnvio = null, Status = StatusCampanha.Rascunho, Criador = "Lucia Fernandes" }
    };

    public List<Campanha> GetAll() => _campanhas;
}
