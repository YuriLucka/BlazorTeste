using BlazorTeste.Application.DTOs;
using FluentValidation;

namespace BlazorTeste.Application.Validators;

public class GerarRelatorioRequestValidator : AbstractValidator<GerarRelatorioRequest>
{
    private static readonly string[] FormatosValidos = ["PDF", "XLSX", "CSV"];

    public GerarRelatorioRequestValidator()
    {
        RuleFor(r => r.Formato)
            .Must(f => f is not null && FormatosValidos.Contains(f.ToUpperInvariant()))
            .WithMessage($"Formato deve ser um de: {string.Join(", ", FormatosValidos)}.");

        RuleFor(r => r.DataFim)
            .GreaterThanOrEqualTo(r => r.DataInicio!.Value)
            .When(r => r.DataInicio.HasValue && r.DataFim.HasValue)
            .WithMessage("Data fim não pode ser anterior à data início.");
    }
}
