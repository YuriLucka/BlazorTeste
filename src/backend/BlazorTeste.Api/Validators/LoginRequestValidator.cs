using BlazorTeste.Api.Models;
using FluentValidation;

namespace BlazorTeste.Api.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(r => r.Senha)
            .NotEmpty().WithMessage("Senha é obrigatória.");
    }
}
