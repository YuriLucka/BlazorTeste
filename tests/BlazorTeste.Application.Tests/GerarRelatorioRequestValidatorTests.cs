using BlazorTeste.Application.DTOs;
using BlazorTeste.Application.Validators;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace BlazorTeste.Application.Tests;

public class GerarRelatorioRequestValidatorTests
{
    private readonly GerarRelatorioRequestValidator _sut = new();

    [Theory]
    [InlineData("PDF")]
    [InlineData("excel")]
    [InlineData("Excel")]
    public void Validate_FormatoValido_NaoRetornaErro(string formato)
    {
        var request = new GerarRelatorioRequest { Formato = formato };

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.Formato);
    }

    [Fact]
    public void Validate_FormatoInvalido_RetornaErro()
    {
        var request = new GerarRelatorioRequest { Formato = "DOCX" };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Formato);
    }

    [Fact]
    public void Validate_FormatoNulo_RetornaErro()
    {
        var request = new GerarRelatorioRequest { Formato = null! };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.Formato);
    }

    [Fact]
    public void Validate_DataFimAntesDataInicio_RetornaErro()
    {
        var request = new GerarRelatorioRequest
        {
            Formato = "PDF",
            DataInicio = new DateTime(2026, 6, 1),
            DataFim = new DateTime(2026, 5, 1)
        };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(r => r.DataFim);
    }

    [Fact]
    public void Validate_DataFimIgualDataInicio_NaoRetornaErro()
    {
        var data = new DateTime(2026, 6, 1);
        var request = new GerarRelatorioRequest { Formato = "PDF", DataInicio = data, DataFim = data };

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.DataFim);
    }

    [Fact]
    public void Validate_DatasNulas_NaoRetornaErro()
    {
        var request = new GerarRelatorioRequest { Formato = "PDF" };

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(r => r.DataFim);
        result.ShouldNotHaveValidationErrorFor(r => r.DataInicio);
    }
}
