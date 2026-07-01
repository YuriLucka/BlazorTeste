# Migração pra ASP.NET Core Identity + autenticação de dois fatores (TOTP) — design

## Contexto

Auth hoje é 100% custom: `Usuario` (`src/backend/BlazorTeste.Domain/Entities/Usuario.cs`) é entidade própria com `Id int`, `SenhaHash`, `RefreshToken`/`RefreshTokenExpiry`, e `List<PermissaoEntidade>` (permissões por entidade/módulo, multi-tenant). `AuthAppService` (`src/backend/BlazorTeste.Application/Services/Implementations/AuthAppService.cs`) faz hash/verify via `IPasswordHasher` custom (`PasswordHelper`) e emite JWT manualmente com `JwtSecurityTokenHandler`. `Program.cs` registra só `AddJwtBearer`, sem Identity. Front (`JwtAuthStateProvider`, `Login.razor`) consome esse JWT via Blazor WASM.

Só existe uma migration EF (`20260630120620_Initial.cs`), ainda não em produção — sem dado real além do seed (`SeedData.Initialize`, senha padrão `Senha@123` pra todos os usuários).

Objetivo: adotar ASP.NET Core Identity — motivação principal é ganhar 2FA (TOTP, tipo Google Authenticator) sem reimplementar na mão.

## Decisões

- **Migração completa pra Identity**, não um 2FA isolado plugado no auth atual — decisão do usuário mesmo sabendo do maior escopo.
- **`ApplicationUser : IdentityUser<Guid>`** — troca `Usuario.Id` de `int` pra `Guid`. Nenhuma outra tabela referencia `Usuario.Id` como FK hoje (confirmado via grep), então o impacto do troca de tipo fica contido nessa entidade.
- **Reset geral de senha** dos usuários seed — hash custom (`PasswordHelper`) não é compatível com `PasswordHasher<TUser>` do Identity, e não há usuário real em produção. Sem código de compatibilidade/fallback.
- **Migration `Initial` substituída** (não incremental) — dropa DB local, gera schema novo do zero com tabelas Identity (`AspNetUsers`, `AspNetUserTokens` etc). Só uma migration existia e não estava em produção.
- **Roles/Claims do Identity fora de escopo** — `PermissaoEntidade` continua como está (claim JSON serializado no JWT, igual hoje), não migra pra `IdentityRole`.

## Arquitetura

### Backend

`ApplicationUser : IdentityUser<Guid>` substitui `Usuario`, mantendo como propriedades extras:
- `Nome` (string)
- `UltimoAcesso` (DateTime)
- `Permissoes` (List\<PermissaoEntidade\>, owned type, inalterado)
- `RefreshToken` / `RefreshTokenExpiry` (Identity não tem conceito de refresh token nativo pra API JWT — continuam campos próprios)

`AppDbContext` passa a herdar de `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`.

`AuthAppService` e `IPasswordHasher`/`PasswordHasher` (Infrastructure) são removidos. `IAuthAppService` continua existindo como fachada (usado pelo `AuthController`), mas a implementação passa a usar `UserManager<ApplicationUser>` e `SignInManager<ApplicationUser>` injetados.

`Program.cs`:
```csharp
builder.Services.AddIdentityCore<ApplicationUser>(opt =>
    {
        opt.Password.RequiredLength = 8; // manter regra atual, ajustar se PasswordHelper tinha outra
        opt.Lockout.MaxFailedAccessAttempts = 5;
        opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddMemoryCache();
```
(`AddJwtBearer` continua como está hoje — token emitido manualmente, Identity só cuida de senha/2FA/lockout, não de emissão de token.)

### Fluxo de login com 2FA

1. `POST /api/auth/login` (email+senha):
   - `SignInManager.CheckPasswordSignInAsync(user, senha, lockoutOnFailure: true)`.
   - Falhou → 401 (como hoje).
   - Ok e `user.TwoFactorEnabled == false` → emite JWT normal (mesma lógica de hoje, `GenerateAccessToken` movida pra dentro do novo `AuthAppService`).
   - Ok e `user.TwoFactorEnabled == true` → **não** emite JWT. Gera `Guid` opaco, guarda em `IMemoryCache` (chave `$"mfa:{guid}"` → `user.Id`, expiração absoluta 5 min). Retorna `202` com `{ requiresTwoFactor: true, mfaToken: "<guid>" }`.
2. `POST /api/auth/login/2fa` (`mfaToken` + `code`):
   - Busca `userId` no cache pelo `mfaToken`; não achou/expirou → `400` "Sessão de verificação expirada, faça login novamente.".
   - Achou → `UserManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code)`.
   - Inválido → `400` "Código inválido." (conta como falha pro lockout via `UserManager.AccessFailedAsync`).
   - Válido → remove do cache, emite JWT normal.

Token opaco (não-JWT) no passo intermediário é proposital: não é aceito por `[Authorize]`/`AddJwtBearer` em nenhum dos 13 controllers existentes, então nenhum deles precisa de mudança pra ficar seguro contra uso do "meio-login".

### Setup de 2FA — nova página "Minha Conta"

Endpoints novos em `AuthController` (autenticados, usuário atual via `ClaimsPrincipal`):
- `POST /api/auth/2fa/setup` → `UserManager.ResetAuthenticatorKeyAsync` + `GetAuthenticatorKeyAsync`, monta `otpauth://totp/SindERP:{email}?secret={key}&issuer=SindERP`, gera PNG via `QRCoder` (`QRCodeGenerator` → `PngByteQRCode`), retorna base64.
- `POST /api/auth/2fa/enable` (`code`) → `VerifyTwoFactorTokenAsync`; ok → `SetTwoFactorEnabledAsync(true)`.
- `POST /api/auth/2fa/disable` (`code` ou senha atual) → confirma e `SetTwoFactorEnabledAsync(false)`.

Frontend: nova página `/minha-conta` (`Components/Pages/Auth/MinhaConta.razor` ou similar), seção "Segurança" com toggle 2FA — desligado mostra botão "Ativar", ativa mostra QR (`<img src="data:image/png;base64,...">`) + campo de código pra confirmar; ligado mostra botão "Desativar" (pede código).

`Login.razor` ganha segundo passo: resposta com `requiresTwoFactor: true` troca o form de senha por campo de código (6 dígitos) + botão "Verificar", chamando `/api/auth/login/2fa` com o `mfaToken` guardado em variável local do componente.

## Pacotes novos

- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (backend, `BlazorTeste.Infrastructure`)
- `QRCoder` (backend, `BlazorTeste.Api` ou `Application` — gera PNG do QR, evita JS interop de QR no WASM)

## Seed

`SeedData.Initialize` reescrito pra usar `UserManager.CreateAsync(user, "Senha@123")` em vez de `IPasswordHasher` direto. Usuários seed continuam com `TwoFactorEnabled = false` — ativação é self-service em "Minha Conta".

## Testes / Verificação

Sem projeto de teste automatizado (mesma limitação de trabalhos anteriores nesta sessão). Verificação manual:

1. Login sem 2FA ativado — regressão, deve continuar funcionando igual hoje.
2. Ativar 2FA em "Minha Conta": escanear QR com app autenticador real, confirmar código, `TwoFactorEnabled` vira `true` no banco.
3. Logout, login de novo — deve pedir código; código certo do app autenticador loga; código errado mostra "Código inválido."; 5 códigos errados seguidos → lockout (login bloqueado 15 min mesmo com código certo).
4. Desativar 2FA — login volta a não pedir código.
5. `mfaToken` expirado (esperar >5 min ou reiniciar API, que limpa o `IMemoryCache`) → mensagem de sessão expirada, força novo login.

## Fora de escopo

- Roles/Claims do Identity (`IdentityRole`) — `PermissaoEntidade` custom continua como está.
- Compatibilidade com hash de senha antigo (`PasswordHelper`) — reset geral, sem fallback.
- 2FA por email/SMS — só TOTP (app autenticador).
- Recovery codes (códigos de backup caso perca o autenticador) — pode ser trabalho futuro, não pedido nessa rodada.
- Qualquer mudança nos 13 controllers de domínio (`ContribuintesController` etc.) — token opaco intermediário já garante que não ficam expostos ao "meio-login" sem precisar de policy nova.
