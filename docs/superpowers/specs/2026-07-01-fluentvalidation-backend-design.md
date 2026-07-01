# FluentValidation no backend — design

## Contexto

Hoje não há validação server-side real. Controllers são finos e passam direto pro Application service; Domain não tem invariantes (`throw`); DTOs não têm DataAnnotations. Frontend valida via atributos MudBlazor (`Required`, `MaxLength`), mas isso não protege a API.

A API atualmente só tem 2 endpoints que recebem corpo (`[FromBody]`):
- `POST /api/auth/login` → `LoginRequest` (definido em `BlazorTeste.Api.Models`)
- `POST /api/relatorios/{id}/gerar` → `GerarRelatorioRequest` (definido em `BlazorTeste.Application.DTOs`)

Todo o resto da API é leitura (GET). Não existem ainda endpoints de create/update para Contribuinte, Cobrança, etc — vão ser adicionados depois, e devem nascer já com validação.

## Objetivo

Adicionar FluentValidation como padrão de validação de entrada, cobrindo os 2 endpoints existentes com corpo, e deixar o pipeline pronto (auto-descoberta de validators via DI) pra próximos endpoints não precisarem de trabalho extra de wiring.

## Pacotes

- `BlazorTeste.Application.csproj` → `FluentValidation` (core) — pacote pequeno, sem acoplamento a ASP.NET, adequado pra camada de aplicação.
- `BlazorTeste.Api.csproj` → `FluentValidation` + `FluentValidation.DependencyInjectionExtensions` — o segundo pacote fornece `AddValidatorsFromAssemblyContaining<T>()`, usado só no `Program.cs`.

Não usar `FluentValidation.AspNetCore` — pacote descontinuado pelo mantenedor desde a mudança do model binding no ASP.NET Core.

## Validators

### `Application/Validators/GerarRelatorioRequestValidator.cs`
- `Formato`: deve ser um de `PDF`, `XLSX`, `CSV` (case-insensitive).
- `DataFim >= DataInicio` quando ambos preenchidos.

### `Api/Validators/LoginRequestValidator.cs`
- `Email`: obrigatório, formato de e-mail válido.
- `Senha`: obrigatória (sem regra de tamanho/complexidade — isso é política de negócio separada, fora de escopo aqui).

## Pipeline de execução

Sem `FluentValidation.AspNetCore`, a integração com o pipeline MVC é manual via um `IAsyncActionFilter` global:

`Api/Filters/FluentValidationFilter.cs`:
1. Para cada argumento em `context.ActionArguments`, resolve `IValidator<T>` do DI container pro tipo do argumento (via `typeof(IValidator<>).MakeGenericType(argType)`).
2. Se existir validator registrado, roda `ValidateAsync`.
3. Erros viram `ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage)`.
4. Se `!ModelState.IsValid` ao final, `context.Result = new BadRequestObjectResult(new ValidationProblemDetails(ModelState))` e não chama `next()`.
5. Caso contrário, `await next()`.

Isso produz resposta 400 `application/problem+json` no mesmo formato que o `[ApiController]` já geraria pra falhas de model binding — consistência sem reinventar formato de erro.

### Registro em `Program.cs`

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<GerarRelatorioRequestValidator>(); // Application
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();          // Api

builder.Services.AddControllers(opt => opt.Filters.Add<FluentValidationFilter>())
    .AddJsonOptions(...); // configuração existente mantida
```

## Testes

- `GerarRelatorioRequestValidator`: unit test novo em `tests/BlazorTeste.Application.Tests/` (projeto já tem xunit + FluentAssertions), cobrindo: formato inválido rejeitado, formato válido passa, `DataFim < DataInicio` rejeitado, datas nulas passam (campos opcionais).
- `LoginRequestValidator`: não há projeto `Api.Tests` hoje e criar um só pra isso é escopo desnecessário. Verificação feita manualmente via request HTTP (login com email vazio/inválido deve retornar 400) depois da implementação.

## Fora de escopo

- Validação de domínio (invariantes em entidades) — não faz parte desta tarefa.
- Endpoints de create/update ainda não existentes (Contribuinte, Cobrança, etc) — o pipeline fica pronto, mas os validators desses DTOs são adicionados quando os endpoints forem criados.
- Regras de senha (tamanho mínimo, complexidade) — política de negócio separada.
