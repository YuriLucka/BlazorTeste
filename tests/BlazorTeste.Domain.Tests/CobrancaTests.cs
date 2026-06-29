using BlazorTeste.Domain.Entities;
using BlazorTeste.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace BlazorTeste.Domain.Tests;

public class CobrancaTests
{
    [Fact]
    public void Pagar_SetaStatusPagoEDataPagamento()
    {
        var cobranca = new Cobranca { Status = StatusCobranca.Pendente };
        var dataPagamento = new DateTime(2026, 6, 15);

        cobranca.Pagar(dataPagamento);

        cobranca.Status.Should().Be(StatusCobranca.Pago);
        cobranca.DataPagamento.Should().Be(dataPagamento);
    }

    [Fact]
    public void Pagar_SobreescreveDataPagamentoAnterior()
    {
        var cobranca = new Cobranca
        {
            Status = StatusCobranca.Pendente,
            DataPagamento = new DateTime(2026, 1, 1)
        };

        cobranca.Pagar(new DateTime(2026, 6, 29));

        cobranca.DataPagamento.Should().Be(new DateTime(2026, 6, 29));
    }

    [Fact]
    public void Vencer_QuandoPendente_SetaStatusVencido()
    {
        var cobranca = new Cobranca { Status = StatusCobranca.Pendente };

        cobranca.Vencer();

        cobranca.Status.Should().Be(StatusCobranca.Vencido);
    }

    [Fact]
    public void Vencer_QuandoPago_NaoAlteraStatus()
    {
        var cobranca = new Cobranca { Status = StatusCobranca.Pago };

        cobranca.Vencer();

        cobranca.Status.Should().Be(StatusCobranca.Pago);
    }

    [Fact]
    public void Vencer_QuandoJaVencido_MantemVencido()
    {
        var cobranca = new Cobranca { Status = StatusCobranca.Vencido };

        cobranca.Vencer();

        cobranca.Status.Should().Be(StatusCobranca.Vencido);
    }

    [Fact]
    public void Vencer_QuandoCancelado_NaoAlteraStatus()
    {
        var cobranca = new Cobranca { Status = StatusCobranca.Cancelado };

        cobranca.Vencer();

        cobranca.Status.Should().Be(StatusCobranca.Cancelado);
    }
}
