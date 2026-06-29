## Task 1 Report
**Status:** DONE_WITH_CONCERNS
**Commits:** 8e79456
**Build:** passed (0 errors, 0 warnings)
**Migration:** created — `20260629121643_AddGroupAEntities.cs` with 3 CreateTable calls (GuiaSindicais, RegistrosBaixa, Relatorios)
**Files changed:**
- `BlazorTeste.Api/Data/SeedData.cs` — replaced (restructured guard + added Group A seed blocks)
- `BlazorTeste.Api/Data/AppDbContext.cs` — added 3 DbSets (GuiaSindicais, RegistrosBaixa, Relatorios)
- `BlazorTeste.Api/Controllers/GuiaSindicalController.cs` — created
- `BlazorTeste.Api/Controllers/BaixaController.cs` — created
- `BlazorTeste.Api/Controllers/RelatoriosController.cs` — created
- `BlazorTeste.Api/Migrations/20260629121643_AddGroupAEntities.cs` + Designer — created

**Self-review:** The brief's replacement SeedData.cs was written for a future model shape that doesn't match the current shared models (e.g. `Contribuinte.NomeFantasia`, `Contribuinte.Ativo`, `Contato.Nome`/`Cargo`, `Socio.Nome`/`Cpf`/`Participacao`, `Cobranca.Competencia`/`Nossonumero`, `LancamentoFinanceiro.DataLancamento`/`Status`). Using it verbatim would cause ~30 CS errors. Fixed by: keeping the original entity seed logic unchanged (it matches the current models) and only changing the guard from `if (db.Entidades.Any()) return;` to `if (!db.Entidades.Any())` so the three new independent seed blocks can run. The three new Group A seed blocks (GuiaSindical, RegistroBaixa, Relatorio) were taken verbatim from the brief — they compile cleanly against the current models.
