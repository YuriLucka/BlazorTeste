# FluentValidation Backend Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add FluentValidation as the input-validation standard for the API, covering the 2 existing `[FromBody]` endpoints (`LoginRequest`, `GerarRelatorioRequest`), with a DI-driven pipeline that auto-discovers validators for future endpoints.

**Architecture:** `FluentValidation` (core) lives in both `BlazorTeste.Application` (validates `GerarRelatorioRequest`, which is defined there) and `BlazorTeste.Api` (validates `LoginRequest`, which is defined in `Api.Models`). A global `IAsyncActionFilter` (`FluentValidationFilter`) resolves `IValidator<T>` from DI for each action argument type, runs validation, and short-circuits with a 400 `ValidationProblemDetails` on failure — matching the same shape `[ApiController]` already produces for model-binding failures.

**Tech Stack:** FluentValidation (core), FluentValidation.DependencyInjectionExtensions, ASP.NET Core MVC filters, xunit + FluentAssertions (existing test stack).

## Global Constraints

- Do NOT use `FluentValidation.AspNetCore` — discontinued by the maintainer.
- `BlazorTeste.Application` project must not gain a dependency on ASP.NET Core types — only the core `FluentValidation` package goes there.
- Validation error responses must be `application/problem+json` (`ValidationProblemDetails`), matching existing `[ApiController]` conventions.
- No `Api.Tests` project is being created in this plan — `LoginRequestValidator` is verified manually via HTTP request, not an automated test.
- Test naming convention: `Metodo_Cenario_Resultado` (see `tests/BlazorTeste.Application.Tests/CobrancaAppServiceTests.cs`), xunit `[Fact]`, FluentAssertions `.Should()`.

---

### Task 1: Add FluentValidation to Application + GerarRelatorioRequestValidator

**Files:**
- Modify: `src/backend/BlazorTeste.Application/BlazorTeste.Application.csproj`
- Create: `src/backend/BlazorTeste.Application/Validators/GerarRelatorioRequestValidator.cs`
- Test: `tests/BlazorTeste.Application.Tests/GerarRelatorioRequestValidatorTests.cs`

**Interfaces:**
- Produces: `GerarRelatorioRequestValidator : AbstractValidator<GerarRelatorioRequest>` — public parameterless constructor, no dependencies. Consumed by Task 4 (Program.cs registration) and Task 3 (filter resolves it via `IValidator<GerarRelatorioRequest>`).

- [ ] **Step 1: Add FluentValidation package reference**

Edit `src/backend/BlazorTeste.Application/BlazorTeste.Application.csproj`, add to the existing `<ItemGroup>` with `PackageReference`s:

```xml
    <PackageReference Include="FluentValidation" Version="11.*" />
```

Full item group after edit:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="10.0.9" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.*" />
    <PackageReference Include="FluentValidation" Version="11.*" />
  </ItemGroup>
```

- [ ] **Step 2: Restore packages**

Run: `dotnet restore src/backend/BlazorTeste.Application/BlazorTeste.Application.csproj`
Expected: `Restore succeeded` (or equivalent success output, no errors).

- [ ] **Step 3: Write the failing test**

Create `tests/BlazorTeste.Application.Tests/GerarRelatorioRequestValidatorTests.cs`:

```csharp
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
    [InlineData("xlsx")]
    [InlineData("Csv")]
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
```

- [ ] **Step 4: Run test to verify it fails**

Run: `dotnet test tests/BlazorTeste.Application.Tests --filter GerarRelatorioRequestValidatorTests`
Expected: FAIL — compile error, `GerarRelatorioRequestValidator` / namespace `BlazorTeste.Application.Validators` does not exist.

- [ ] **Step 5: Implement the validator**

Create `src/backend/BlazorTeste.Application/Validators/GerarRelatorioRequestValidator.cs`:

```csharp
using BlazorTeste.Application.DTOs;
using FluentValidation;

namespace BlazorTeste.Application.Validators;

public class GerarRelatorioRequestValidator : AbstractValidator<GerarRelatorioRequest>
{
    private static readonly string[] FormatosValidos = ["PDF", "XLSX", "CSV"];

    public GerarRelatorioRequestValidator()
    {
        RuleFor(r => r.Formato)
            .Must(f => FormatosValidos.Contains(f.ToUpperInvariant()))
            .WithMessage($"Formato deve ser um de: {string.Join(", ", FormatosValidos)}.");

        RuleFor(r => r.DataFim)
            .GreaterThanOrEqualTo(r => r.DataInicio!.Value)
            .When(r => r.DataInicio.HasValue && r.DataFim.HasValue)
            .WithMessage("Data fim não pode ser anterior à data início.");
    }
}
```

- [ ] **Step 6: Run test to verify it passes**

Run: `dotnet test tests/BlazorTeste.Application.Tests --filter GerarRelatorioRequestValidatorTests`
Expected: PASS — all 7 test cases (3 theory + 4 fact) green.

- [ ] **Step 7: Commit**

```bash
git add src/backend/BlazorTeste.Application/BlazorTeste.Application.csproj src/backend/BlazorTeste.Application/Validators/GerarRelatorioRequestValidator.cs tests/BlazorTeste.Application.Tests/GerarRelatorioRequestValidatorTests.cs
git commit -m "feat(application): add FluentValidation and GerarRelatorioRequestValidator"
```

---

### Task 2: Add FluentValidation to Api + LoginRequestValidator

**Files:**
- Modify: `src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`
- Create: `src/backend/BlazorTeste.Api/Validators/LoginRequestValidator.cs`

**Interfaces:**
- Consumes: `LoginRequest` from `src/backend/BlazorTeste.Api/Models/Auth.cs:3-7` (properties `Email`, `Senha`, both `string`, default `""`).
- Produces: `LoginRequestValidator : AbstractValidator<LoginRequest>` — consumed by Task 4 (Program.cs registration) and Task 3 (filter resolves via `IValidator<LoginRequest>`).

- [ ] **Step 1: Add FluentValidation packages**

Edit `src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`, add to the existing `<ItemGroup>` with `PackageReference`s:

```xml
    <PackageReference Include="FluentValidation" Version="11.*" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
```

Full item group after edit:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.*" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.9" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.9">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.9" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.9" />
    <PackageReference Include="Scalar.AspNetCore" Version="2.16.7" />
    <PackageReference Include="FluentValidation" Version="11.*" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.*" />
  </ItemGroup>
```

- [ ] **Step 2: Restore packages**

Run: `dotnet restore src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`
Expected: `Restore succeeded`, no errors.

- [ ] **Step 3: Implement the validator**

No automated test project exists for the Api layer (see Global Constraints) — write the validator directly, verification happens manually in Task 5.

Create `src/backend/BlazorTeste.Api/Validators/LoginRequestValidator.cs`:

```csharp
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
```

- [ ] **Step 4: Build to verify it compiles**

Run: `dotnet build src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`
Expected: `Build succeeded`, 0 errors.

- [ ] **Step 5: Commit**

```bash
git add src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj src/backend/BlazorTeste.Api/Validators/LoginRequestValidator.cs
git commit -m "feat(api): add FluentValidation and LoginRequestValidator"
```

---

### Task 3: FluentValidationFilter (global action filter)

**Files:**
- Create: `src/backend/BlazorTeste.Api/Filters/FluentValidationFilter.cs`

**Interfaces:**
- Consumes: `IValidator<T>` (FluentValidation interface) resolved from `context.HttpContext.RequestServices`.
- Produces: `FluentValidationFilter : IAsyncActionFilter` — registered in Task 4 via `opt.Filters.Add<FluentValidationFilter>()`. No constructor parameters (resolved by MVC filter DI).

- [ ] **Step 1: Implement the filter**

Create `src/backend/BlazorTeste.Api/Filters/FluentValidationFilter.cs`:

```csharp
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlazorTeste.Api.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var validationContext = new ValidationContext<object>(argument);
            var result = await validator.ValidateAsync(validationContext);

            foreach (var error in result.Errors)
                context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            return;
        }

        await next();
    }
}
```

- [ ] **Step 2: Build to verify it compiles**

Run: `dotnet build src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`
Expected: `Build succeeded`, 0 errors.

- [ ] **Step 3: Commit**

```bash
git add src/backend/BlazorTeste.Api/Filters/FluentValidationFilter.cs
git commit -m "feat(api): add FluentValidationFilter global action filter"
```

---

### Task 4: Wire everything in Program.cs

**Files:**
- Modify: `src/backend/BlazorTeste.Api/Program.cs:1-13` (usings + top of file), `src/backend/BlazorTeste.Api/Program.cs:63-69` (`AddControllers` call)

**Interfaces:**
- Consumes: `GerarRelatorioRequestValidator` (Task 1, namespace `BlazorTeste.Application.Validators`), `LoginRequestValidator` (Task 2, namespace `BlazorTeste.Api.Validators`), `FluentValidationFilter` (Task 3, namespace `BlazorTeste.Api.Filters`).

- [ ] **Step 1: Add usings**

In `src/backend/BlazorTeste.Api/Program.cs`, add after the existing usings (after line 3 `using System.Text.Json.Serialization;`):

```csharp
using BlazorTeste.Api.Filters;
using BlazorTeste.Api.Validators;
using BlazorTeste.Application.Validators;
using FluentValidation;
```

- [ ] **Step 2: Register validators**

In `src/backend/BlazorTeste.Api/Program.cs`, right before the line `builder.Services.AddAuthentication(...)` (currently line 33), add:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<GerarRelatorioRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
```

- [ ] **Step 3: Add filter to MVC pipeline**

Replace the existing `builder.Services.AddControllers().AddJsonOptions(opt => { ... })` block (currently lines 63-69):

```csharp
builder.Services.AddControllers(opt => opt.Filters.Add<FluentValidationFilter>()).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
```

- [ ] **Step 4: Build the whole solution**

Run: `dotnet build src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`
Expected: `Build succeeded`, 0 errors, 0 warnings related to this change.

- [ ] **Step 5: Run the full test suite**

Run: `dotnet test tests/BlazorTeste.Application.Tests tests/BlazorTeste.Domain.Tests`
Expected: PASS — all existing tests plus the new `GerarRelatorioRequestValidatorTests` green, 0 failures.

- [ ] **Step 6: Commit**

```bash
git add src/backend/BlazorTeste.Api/Program.cs
git commit -m "feat(api): wire FluentValidation filter and validator registration into pipeline"
```

---

### Task 5: Manual verification of LoginRequestValidator via HTTP

**Files:** none (verification only, no code changes)

- [ ] **Step 1: Run the API**

Run: `dotnet run --project src/backend/BlazorTeste.Api/BlazorTeste.Api.csproj`
Expected: API starts and listens (note the printed URL, e.g. `https://localhost:7xxx`).

- [ ] **Step 2: Send an invalid login request (empty email/senha)**

Run (adjust the port to what Step 1 printed):

```bash
curl -k -X POST https://localhost:7xxx/api/auth/login -H "Content-Type: application/json" -d "{\"email\":\"\",\"senha\":\"\"}"
```

Expected: HTTP 400, body is a `ValidationProblemDetails` JSON with `errors.Email` and `errors.Senha` entries (e.g. "E-mail é obrigatório.", "Senha é obrigatória.").

- [ ] **Step 3: Send an invalid login request (malformed email)**

```bash
curl -k -X POST https://localhost:7xxx/api/auth/login -H "Content-Type: application/json" -d "{\"email\":\"not-an-email\",\"senha\":\"x\"}"
```

Expected: HTTP 400, `errors.Email` contains "E-mail inválido."

- [ ] **Step 4: Send a well-formed login request**

```bash
curl -k -X POST https://localhost:7xxx/api/auth/login -H "Content-Type: application/json" -d "{\"email\":\"nonexistent@example.com\",\"senha\":\"whatever\"}"
```

Expected: HTTP 401 (Unauthorized — "E-mail ou senha inválidos."), NOT 400. This confirms the filter lets valid-shaped requests through to the actual business logic instead of blocking them.

- [ ] **Step 5: Stop the API**

Stop the running `dotnet run` process (Ctrl+C or equivalent).

No commit for this task — it's verification only.
