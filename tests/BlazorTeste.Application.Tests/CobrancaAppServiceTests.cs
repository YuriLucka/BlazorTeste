using BlazorTeste.Application.Services.Implementations;
using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using BlazorTeste.Domain.Interfaces.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BlazorTeste.Application.Tests;

public class CobrancaAppServiceTests
{
    private readonly ICobrancaRepository _repo = Substitute.For<ICobrancaRepository>();
    private readonly CobrancaAppService _sut;

    public CobrancaAppServiceTests() => _sut = new CobrancaAppService(_repo);

    private static Cobranca MakeCobranca(int id, int entidadeId, StatusCobranca status = StatusCobranca.Pendente) =>
        new()
        {
            Id = id,
            EntidadeId = entidadeId,
            ContribuinteId = 1,
            RazaoSocialContribuinte = $"Empresa {id}",
            Tipo = TipoCobranca.Sindical,
            Valor = 1000m,
            Vencimento = new DateTime(2026, 6, 30),
            Status = status,
            AnoReferencia = 2026
        };

    [Fact]
    public async Task GetAllAsync_QuandoEntidadeIdZero_RetornaTodas()
    {
        var cobrancas = new List<Cobranca>
        {
            MakeCobranca(1, 1),
            MakeCobranca(2, 2)
        };
        _repo.GetAllAsync().Returns(cobrancas);

        var result = await _sut.GetAllAsync(0);

        result.Should().HaveCount(2);
        await _repo.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetAllAsync_QuandoEntidadeIdFornecido_FiltrarPorEntidade()
    {
        var cobrancas = new List<Cobranca> { MakeCobranca(1, 3) };
        _repo.GetByEntidadeAsync(3).Returns(cobrancas);

        var result = await _sut.GetAllAsync(3);

        result.Should().HaveCount(1);
        result.First().EntidadeId.Should().Be(3);
        await _repo.Received(1).GetByEntidadeAsync(3);
    }

    [Fact]
    public async Task GetAllAsync_MapeiaStatusComoString()
    {
        _repo.GetAllAsync().Returns(new List<Cobranca> { MakeCobranca(1, 1, StatusCobranca.Pago) });

        var result = await _sut.GetAllAsync(0);

        result.Single().Status.Should().Be("Pago");
    }

    [Fact]
    public async Task GetAllAsync_MapeiaValoresMonetariosCorretamente()
    {
        var cobranca = MakeCobranca(1, 1);
        cobranca.Valor = 2500.75m;
        cobranca.Multa = 125.04m;
        cobranca.Juros = 37.51m;
        _repo.GetAllAsync().Returns(new List<Cobranca> { cobranca });

        var result = await _sut.GetAllAsync(0);

        var dto = result.Single();
        dto.Valor.Should().Be(2500.75m);
        dto.Multa.Should().Be(125.04m);
        dto.Juros.Should().Be(37.51m);
    }

    [Fact]
    public async Task GetByStatusAsync_StatusInvalido_RetornaVazio()
    {
        var result = await _sut.GetByStatusAsync(0, "StatusInexistente");

        result.Should().BeEmpty();
        await _repo.DidNotReceive().GetByStatusAsync(Arg.Any<StatusCobranca>());
    }

    [Fact]
    public async Task GetByStatusAsync_StatusValido_RetornaFiltrado()
    {
        var cobrancas = new List<Cobranca>
        {
            MakeCobranca(1, 1, StatusCobranca.Vencido),
            MakeCobranca(2, 1, StatusCobranca.Vencido)
        };
        _repo.GetByStatusAsync(StatusCobranca.Vencido).Returns(cobrancas);

        var result = await _sut.GetByStatusAsync(0, "Vencido");

        result.Should().HaveCount(2);
        result.All(c => c.Status == "Vencido").Should().BeTrue();
    }

    [Fact]
    public async Task GetByStatusAsync_StatusValido_CaseInsensitive()
    {
        _repo.GetByStatusAsync(StatusCobranca.Pago).Returns(new List<Cobranca> { MakeCobranca(1, 1, StatusCobranca.Pago) });

        var result = await _sut.GetByStatusAsync(0, "pago");

        result.Should().HaveCount(1);
    }
}
