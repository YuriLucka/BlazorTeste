## Task 2 Report
**Status:** DONE
**Commits:** 4542e27
**Build:** passed (0 errors, 0 warnings)
**Migration:** created — `20260629122603_AddGroupBEntities` with tables: Configuracoes, Eventos, Negociacoes, EventoInscricoes, NegociacaoCobrancasOriginais, NegociacaoParcelas
**Files changed:**
- `BlazorTeste.Shared/Models/Negociacao.cs` — added `Id` to `ParcelaNegociacao` and `CobrancaOriginalNeg`
- `BlazorTeste.Shared/Models/Configuracao.cs` — added `ConfiguracaoEntidade` aggregate class
- `BlazorTeste.Api/Data/AppDbContext.cs` — added 3 DbSets + EF OwnsMany/OwnsOne/ToJson config
- `BlazorTeste.Api/Data/SeedData.cs` — added Negociacoes, Eventos, Configuracoes seed blocks
- `BlazorTeste.Api/Controllers/NegociacoesController.cs` — new (GET api/negociacoes)
- `BlazorTeste.Api/Controllers/EventosController.cs` — new (GET api/eventos)
- `BlazorTeste.Api/Controllers/ConfiguracoesController.cs` — new (GET api/configuracoes/geral|cobranca|email|banco)
- `BlazorTeste.Api/Migrations/20260629122603_AddGroupBEntities.cs` — new migration
- `BlazorTeste.Api/Migrations/20260629122603_AddGroupBEntities.Designer.cs` — new migration snapshot
- `BlazorTeste.Api/Migrations/AppDbContextModelSnapshot.cs` — updated by EF tooling
**Self-review:** Initial build failed due to the API process locking the shared DLL (Visual Studio had the app running). Killed the process and rebuild succeeded cleanly. The `o.OwnsMany(cc => cc.Faixas)` inside `ToJson()` was kept as-is per the brief — migration generated without errors, confirming EF Core 10 handles it correctly. No code issues found.
