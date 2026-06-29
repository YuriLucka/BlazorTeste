using BlazorTeste.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace BlazorTeste.Domain.Tests;

public class ProcessoTests
{
    [Fact]
    public void Encerrar_SetaSituacaoEncerrado()
    {
        var processo = new Processo { Situacao = "Em andamento" };

        processo.Encerrar();

        processo.Situacao.Should().Be("Encerrado");
    }

    [Fact]
    public void Suspender_SetaSituacaoSuspenso()
    {
        var processo = new Processo { Situacao = "Em andamento" };

        processo.Suspender();

        processo.Situacao.Should().Be("Suspenso");
    }

    [Fact]
    public void Encerrar_SobreescreveSituacaoAnterior()
    {
        var processo = new Processo { Situacao = "Suspenso" };

        processo.Encerrar();

        processo.Situacao.Should().Be("Encerrado");
    }

    [Fact]
    public void Suspender_SobreescreveSituacaoAnterior()
    {
        var processo = new Processo { Situacao = "Encerrado" };

        processo.Suspender();

        processo.Situacao.Should().Be("Suspenso");
    }
}
