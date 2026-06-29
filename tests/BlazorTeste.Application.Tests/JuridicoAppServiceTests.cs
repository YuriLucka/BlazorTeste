using BlazorTeste.Application.Services.Implementations;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Interfaces.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BlazorTeste.Application.Tests;

public class JuridicoAppServiceTests
{
    private readonly IJuridicoRepository _repo = Substitute.For<IJuridicoRepository>();
    private readonly JuridicoAppService _sut;

    public JuridicoAppServiceTests() => _sut = new JuridicoAppService(_repo);

    [Fact]
    public async Task GetProcessosAsync_QuandoEntidadeIdZero_RetornaTodos()
    {
        var processos = new List<Processo>
        {
            new() { Id = 1, EntidadeId = 1, Numero = "001", Situacao = "Em andamento" },
            new() { Id = 2, EntidadeId = 2, Numero = "002", Situacao = "Encerrado" }
        };
        _repo.GetAllAsync().Returns(processos);

        var result = await _sut.GetProcessosAsync(0);

        result.Should().HaveCount(2);
        await _repo.Received(1).GetAllAsync();
        await _repo.DidNotReceive().GetByEntidadeAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetProcessosAsync_QuandoEntidadeIdFornecido_FiltrarPorEntidade()
    {
        var processos = new List<Processo>
        {
            new() { Id = 1, EntidadeId = 1, Numero = "001", Situacao = "Em andamento" }
        };
        _repo.GetByEntidadeAsync(1).Returns(processos);

        var result = await _sut.GetProcessosAsync(1);

        result.Should().HaveCount(1);
        result.First().EntidadeId.Should().Be(1);
        await _repo.Received(1).GetByEntidadeAsync(1);
        await _repo.DidNotReceive().GetAllAsync();
    }

    [Fact]
    public async Task GetProcessosAsync_MapeiaPropriedadesCorretamente()
    {
        var processo = new Processo
        {
            Id = 42,
            EntidadeId = 5,
            Numero = "9022-73.2026.5.02.7418",
            Tipo = "Ação civil pública",
            Vara = "1ª Vara do Trabalho",
            Tribunal = "TJSP",
            Situacao = "Em andamento",
            AdvogadoId = 3,
            NomeAdvogado = "Dr. Carlos Eduardo",
            DataAbertura = new DateTime(2026, 1, 10),
            Descricao = "Ação coletiva"
        };
        _repo.GetAllAsync().Returns(new List<Processo> { processo });

        var result = (await _sut.GetProcessosAsync(0)).Single();

        result.Id.Should().Be(42);
        result.EntidadeId.Should().Be(5);
        result.Numero.Should().Be("9022-73.2026.5.02.7418");
        result.Tipo.Should().Be("Ação civil pública");
        result.Tribunal.Should().Be("TJSP");
        result.Situacao.Should().Be("Em andamento");
        result.NomeAdvogado.Should().Be("Dr. Carlos Eduardo");
    }

    [Fact]
    public async Task GetAdvogadosAsync_QuandoEntidadeIdZero_RetornaTodos()
    {
        var advogados = new List<Advogado>
        {
            new() { Id = 1, Nome = "Dr. João", Oab = "OAB/SP 111" },
            new() { Id = 2, Nome = "Dra. Maria", Oab = "OAB/SP 222" }
        };
        _repo.GetAllAdvogadosAsync().Returns(advogados);

        var result = await _sut.GetAdvogadosAsync(0);

        result.Should().HaveCount(2);
        await _repo.Received(1).GetAllAdvogadosAsync();
        await _repo.DidNotReceive().GetAdvogadosAsync(Arg.Any<int>());
    }

    [Fact]
    public async Task GetAdvogadosAsync_QuandoEntidadeIdFornecido_FiltrarPorEntidade()
    {
        var advogados = new List<Advogado>
        {
            new() { Id = 1, EntidadeId = 2, Nome = "Dr. João" }
        };
        _repo.GetAdvogadosAsync(2).Returns(advogados);

        var result = await _sut.GetAdvogadosAsync(2);

        result.Should().HaveCount(1);
        await _repo.Received(1).GetAdvogadosAsync(2);
    }

    [Fact]
    public async Task GetAudienciasAsync_RetornaTodas()
    {
        var audiencias = new List<Audiencia>
        {
            new() { Id = 1, ProcessoId = 10, NumeroProcesso = "001", Tipo = "Inicial", Situacao = "Agendada" },
            new() { Id = 2, ProcessoId = 11, NumeroProcesso = "002", Tipo = "Perícia", Situacao = "Realizada" }
        };
        _repo.GetAllAudienciasAsync().Returns(audiencias);

        var result = await _sut.GetAudienciasAsync();

        result.Should().HaveCount(2);
        result.First().Situacao.Should().Be("Agendada");
        result.Last().Situacao.Should().Be("Realizada");
    }

    [Fact]
    public async Task GetAudienciasAsync_MapeiaPropriedadesCorretamente()
    {
        var audiencia = new Audiencia
        {
            Id = 99,
            ProcessoId = 5,
            NumeroProcesso = "5372-53.2026.5.02.2652",
            AdvogadoId = 3,
            NomeAdvogado = "Dr. Carlos Eduardo Mendes",
            DataHora = new DateTime(2026, 8, 1, 9, 0, 0),
            Tipo = "Perícia",
            Local = "TJSP — Pátio do Colégio",
            Situacao = "Agendada"
        };
        _repo.GetAllAudienciasAsync().Returns(new List<Audiencia> { audiencia });

        var result = (await _sut.GetAudienciasAsync()).Single();

        result.Id.Should().Be(99);
        result.ProcessoId.Should().Be(5);
        result.NumeroProcesso.Should().Be("5372-53.2026.5.02.2652");
        result.NomeAdvogado.Should().Be("Dr. Carlos Eduardo Mendes");
        result.Tipo.Should().Be("Perícia");
        result.Local.Should().Be("TJSP — Pátio do Colégio");
        result.Situacao.Should().Be("Agendada");
    }
}
