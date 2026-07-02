# MudBlazor 6→9 Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade the frontend's MudBlazor dependency from 6.21.0 to 9.6.0, incrementally through major versions 7 and 8, fixing each major's confirmed breaking changes before advancing.

**Architecture:** One `.csproj` version bump per major (`7.16.0` → `8.15.0` → `9.6.0`, exact pins), `dotnet build` after each bump to surface compiler + MudBlazor Roslyn-analyzer errors, fix the specific known breakages for that major (verified against actual MudBlazor source at each target tag, not just the migration guide prose), commit, then move to the next major. Manual browser smoke test only after the final v9 bump.

**Tech Stack:** Blazor WebAssembly (.NET 10), MudBlazor, no automated frontend test suite.

## Global Constraints

- Frontend project: `src/frontend/BlazorTeste/BlazorTeste.csproj`, SDK `Microsoft.NET.Sdk.BlazorWebAssembly`, `net10.0`.
- Package versions pinned exact per step: `7.16.0`, `8.15.0`, `9.6.0` — not floating ranges (`7.*` etc). This matches the repo's existing convention of exact pins for packages that need stability during a migration window.
- No automated test suite exists for the frontend (documented, not itself a gap to fix in this plan).
- `Modal` default changing from `true` to `false` on popover-based components (v9) is accepted as-is, no proactive override — only fix concrete regressions the smoke test finds.
- Do not touch `MudDataGrid`, `MudChart`, `MudStepper`, `MudFileUpload`, `MudChat` usage beyond what a clean build requires — no component swaps, this is a version bump only.
- Verified against actual MudBlazor source code at each target git tag (`v7.16.0`, `v8.15.0`, `v9.6.0`), not only the prose migration guides — where this plan's code differs from the design spec's initial draft, the source-verified version is used.

Reference: `docs/superpowers/specs/2026-07-02-mudblazor-9-migration-design.md`.

---

## Task 1: Bump to MudBlazor 7.16.0 and fix v7 breaking changes

**Files:**
- Modify: `src/frontend/BlazorTeste/BlazorTeste.csproj`
- Modify: `src/frontend/BlazorTeste/Components/Layout/MainLayout.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Negociacao/Negociacoes.razor`

**Interfaces:** None — this task only touches package version and component markup, no new types or methods produced for later tasks.

- [ ] **Step 1: Bump the package version**

Current line in `src/frontend/BlazorTeste/BlazorTeste.csproj`:
```xml
    <PackageReference Include="MudBlazor" Version="6.*" />
```

Replace with:
```xml
    <PackageReference Include="MudBlazor" Version="7.16.0" />
```

- [ ] **Step 2: Add the required `MudPopoverProvider`**

Current top of `src/frontend/BlazorTeste/Components/Layout/MainLayout.razor` (lines 12-14):
```razor
<MudThemeProvider Theme="_theme" IsDarkMode="AppState.DarkMode" />
<MudDialogProvider />
<MudSnackbarProvider />
```

Replace with:
```razor
<MudThemeProvider Theme="_theme" IsDarkMode="AppState.DarkMode" />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />
```

- [ ] **Step 3: Rename the theme's `Palette` property to `PaletteLight`**

Current block in `src/frontend/BlazorTeste/Components/Layout/MainLayout.razor` (lines 108-132):
```csharp
    private static readonly MudTheme _theme = new()
    {
        Palette = new PaletteLight
        {
            Primary = "#1565C0",
            Secondary = "#00897B",
            Background = "#F5F7FA",
            AppbarBackground = "#1565C0",
            DrawerBackground = "#FFFFFF"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#5C9CE6",
            Secondary = "#4DB6A6",
            Background = "#1A1A2E",
            Surface = "#16213E",
            AppbarBackground = "#0D47A1",
            DrawerBackground = "#16213E"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "240px"
        }
    };
```

Replace with:
```csharp
    private static readonly MudTheme _theme = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1565C0",
            Secondary = "#00897B",
            Background = "#F5F7FA",
            AppbarBackground = "#1565C0",
            DrawerBackground = "#FFFFFF"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#5C9CE6",
            Secondary = "#4DB6A6",
            Background = "#1A1A2E",
            Surface = "#16213E",
            AppbarBackground = "#0D47A1",
            DrawerBackground = "#16213E"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "240px"
        }
    };
```

- [ ] **Step 4: Rename the `grey`→`gray` CSS variable**

Current line in `src/frontend/BlazorTeste/Components/Pages/Negociacao/Negociacoes.razor:145`:
```razor
                            <tr style="font-weight:700; background:var(--mud-palette-background-grey)">
```

Replace with:
```razor
                            <tr style="font-weight:700; background:var(--mud-palette-background-gray)">
```

- [ ] **Step 5: Restore packages and build**

Run (from `src/frontend/BlazorTeste`): `dotnet restore && dotnet build`
Expected: `Build succeeded.` with 0 errors. If the MudBlazor Roslyn analyzer or the C# compiler reports any additional errors/warnings beyond what Steps 2-4 fixed (e.g. an orphaned Razor attribute from a renamed parameter this plan's grep didn't catch), fix them using the corresponding rename documented in the MudBlazor v7.0.0 migration guide (https://github.com/MudBlazor/MudBlazor/issues/8447) — the fix must match an actual documented v7 rename, not a guess. If a build error doesn't match anything in that guide, stop and report it rather than inventing a fix.

- [ ] **Step 6: Commit**

```bash
git add src/frontend/BlazorTeste/BlazorTeste.csproj src/frontend/BlazorTeste/Components/Layout/MainLayout.razor src/frontend/BlazorTeste/Components/Pages/Negociacao/Negociacoes.razor
git commit -m "chore(frontend): bump MudBlazor 6.21.0 -> 7.16.0"
```

(If Step 5 required fixes beyond Steps 2-4, include those changed files in the `git add` as well.)

---

## Task 2: Bump to MudBlazor 8.15.0 and fix v8 breaking changes

**Files:**
- Modify: `src/frontend/BlazorTeste/BlazorTeste.csproj`
- Modify: `src/frontend/BlazorTeste/Components/Shared/ConfirmDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Cobranca/NovaCobrancaDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/InativarContribuinteDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Evento/NovoEventoDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Financeiro/NovoLancamentoDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/GuiaSindical/EmitirGuiaSindicalDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Juridico/NovoProcessoDialog.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Negociacao/NovaNegociacaoDialog.razor`

**Interfaces:**
- Consumes: nothing from Task 1 beyond the already-bumped package.
- Produces: nothing new for later tasks — `IMudDialogInstance` is a MudBlazor library type, not something this plan defines.

**Important:** `IMudDialogInstance.Close(DialogResult)` and `.Cancel()` are confirmed **synchronous** at both v8.15.0 and v9.6.0 (verified directly against MudBlazor's `IMudDialogInstance.cs` source at those tags). Only the *type name* changes from `MudDialogInstance` to `IMudDialogInstance` in the `[CascadingParameter]` field declaration. Do **not** add `async`/`await`/`Async` suffixes to the `Close`/`Cancel` calls in these files — that would be a compile error since no such overload exists.

- [ ] **Step 1: Bump the package version**

Current line in `src/frontend/BlazorTeste/BlazorTeste.csproj`:
```xml
    <PackageReference Include="MudBlazor" Version="7.16.0" />
```

Replace with:
```xml
    <PackageReference Include="MudBlazor" Version="8.15.0" />
```

- [ ] **Step 2: Rename `MudDialogInstance` to `IMudDialogInstance` in `ConfirmDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Shared/ConfirmDialog.razor:29`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Lines 37-38, `MudDialog.Close(DialogResult.Ok(true));` and `MudDialog.Cancel();`, stay exactly as-is — no change.)

- [ ] **Step 3: Rename `MudDialogInstance` to `IMudDialogInstance` in `NovaCobrancaDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Cobranca/NovaCobrancaDialog.razor:74`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 105, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 4: Rename `MudDialogInstance` to `IMudDialogInstance` in `InativarContribuinteDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/InativarContribuinteDialog.razor:25`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 28, `MudDialog.Close(DialogResult.Ok(_motivo));`, stays exactly as-is — no change.)

- [ ] **Step 5: Rename `MudDialogInstance` to `IMudDialogInstance` in `NovoContribuinteDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor:258`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 308, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 6: Rename `MudDialogInstance` to `IMudDialogInstance` in `NovoEventoDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Evento/NovoEventoDialog.razor:196`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 228, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 7: Rename `MudDialogInstance` to `IMudDialogInstance` in `NovoLancamentoDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Financeiro/NovoLancamentoDialog.razor:148`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 194, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 8: Rename `MudDialogInstance` to `IMudDialogInstance` in `EmitirGuiaSindicalDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/GuiaSindical/EmitirGuiaSindicalDialog.razor:204`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 240, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 9: Rename `MudDialogInstance` to `IMudDialogInstance` in `NovoProcessoDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Juridico/NovoProcessoDialog.razor:113`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 146, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 10: Rename `MudDialogInstance` to `IMudDialogInstance` in `NovaNegociacaoDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Negociacao/NovaNegociacaoDialog.razor:247`:
```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
```

Replace with:
```csharp
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
```

(Line 299, `MudDialog.Close(DialogResult.Ok(true));`, stays exactly as-is — no change.)

- [ ] **Step 11: Restore packages and build**

Run (from `src/frontend/BlazorTeste`): `dotnet restore && dotnet build`
Expected: `Build succeeded.` with 0 errors. If the analyzer/compiler reports additional errors beyond the 9 dialog renames, fix them using the corresponding rename documented in the MudBlazor v8.0.0 migration guide (https://github.com/MudBlazor/MudBlazor/issues/9953) — verify any non-obvious fix against MudBlazor's actual source at the `v8.15.0` git tag (e.g. via `gh api repos/MudBlazor/MudBlazor/contents/<path>?ref=v8.15.0`) before applying it, the way this plan verified the `Close`/`Cancel` signatures. If a build error doesn't match anything in the guide, stop and report it rather than inventing a fix.

- [ ] **Step 12: Commit**

```bash
git add src/frontend/BlazorTeste/BlazorTeste.csproj \
        src/frontend/BlazorTeste/Components/Shared/ConfirmDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Cobranca/NovaCobrancaDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/InativarContribuinteDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Evento/NovoEventoDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Financeiro/NovoLancamentoDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/GuiaSindical/EmitirGuiaSindicalDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Juridico/NovoProcessoDialog.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Negociacao/NovaNegociacaoDialog.razor
git commit -m "chore(frontend): bump MudBlazor 7.16.0 -> 8.15.0"
```

(If Step 11 required fixes in additional files, include those in the `git add` as well.)

---

## Task 3: Bump to MudBlazor 9.6.0 and fix v9 breaking changes

**Files:**
- Modify: `src/frontend/BlazorTeste/BlazorTeste.csproj`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Negociacao/Negociacoes.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Configuracoes/Configuracoes.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Juridico/Juridico.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/GuiaSindical/GuiaSindicais.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Cobranca/Cobrancas.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Contribuintes/Contribuintes.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Baixa/BaixaCobranca.razor`
- Modify: `src/frontend/BlazorTeste/Components/Pages/Financeiro/Financeiro.razor`
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor`

**Interfaces:** None — package bump plus attribute rename on `<MudTabs>`, no new types.

- [ ] **Step 1: Bump the package version**

Current line in `src/frontend/BlazorTeste/BlazorTeste.csproj`:
```xml
    <PackageReference Include="MudBlazor" Version="8.15.0" />
```

Replace with:
```xml
    <PackageReference Include="MudBlazor" Version="9.6.0" />
```

- [ ] **Step 2: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `Negociacoes.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Negociacao/Negociacoes.razor:80`:
```razor
        <MudTabs Elevation="0" PanelClass="pt-3">
```

Replace with:
```razor
        <MudTabs Elevation="0" TabPanelsClass="pt-3">
```

- [ ] **Step 3: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `Configuracoes.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Configuracoes/Configuracoes.razor:22`:
```razor
        <MudTabs Elevation="0" PanelClass="pt-4">
```

Replace with:
```razor
        <MudTabs Elevation="0" TabPanelsClass="pt-4">
```

- [ ] **Step 4: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `Juridico.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Juridico/Juridico.razor:19`:
```razor
        <MudTabs Elevation="0" PanelClass="pt-3">
```

Replace with:
```razor
        <MudTabs Elevation="0" TabPanelsClass="pt-3">
```

- [ ] **Step 5: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `GuiaSindicais.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/GuiaSindical/GuiaSindicais.razor:80`:
```razor
        <MudTabs Elevation="0" PanelClass="pt-3">
```

Replace with:
```razor
        <MudTabs Elevation="0" TabPanelsClass="pt-3">
```

- [ ] **Step 6: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `Cobrancas.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Cobranca/Cobrancas.razor:21`:
```razor
        <MudTabs Elevation="0" Rounded="false" PanelClass="pt-2">
```

Replace with:
```razor
        <MudTabs Elevation="0" Rounded="false" TabPanelsClass="pt-2">
```

- [ ] **Step 7: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `Contribuintes.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Contribuintes/Contribuintes.razor:73`:
```razor
            <MudTabs Elevation="0" Rounded="true" ApplyEffectsToContainer="true" PanelClass="pt-3">
```

Replace with:
```razor
            <MudTabs Elevation="0" Rounded="true" ApplyEffectsToContainer="true" TabPanelsClass="pt-3">
```

- [ ] **Step 8: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `BaixaCobranca.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Baixa/BaixaCobranca.razor:64`:
```razor
        <MudTabs Elevation="0" PanelClass="pt-3">
```

Replace with:
```razor
        <MudTabs Elevation="0" TabPanelsClass="pt-3">
```

- [ ] **Step 9: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `Financeiro.razor`**

Current line in `src/frontend/BlazorTeste/Components/Pages/Financeiro/Financeiro.razor:19`:
```razor
        <MudTabs Elevation="0" PanelClass="pt-3">
```

Replace with:
```razor
        <MudTabs Elevation="0" TabPanelsClass="pt-3">
```

- [ ] **Step 10: Rename `PanelClass` to `TabPanelsClass` on `<MudTabs>` in `NovoContribuinteDialog.razor`**

Current line in `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor:12`:
```razor
        <MudTabs Elevation="0" @bind-ActivePanelIndex="_tab" PanelClass="pt-4" KeepPanelsAlive="true">
```

Replace with:
```razor
        <MudTabs Elevation="0" @bind-ActivePanelIndex="_tab" TabPanelsClass="pt-4" KeepPanelsAlive="true">
```

- [ ] **Step 11: Restore packages and build**

Run (from `src/frontend/BlazorTeste`): `dotnet restore && dotnet build`
Expected: `Build succeeded.` with 0 errors. If the analyzer/compiler reports additional errors beyond the 9 `TabPanelsClass` renames, fix them using the corresponding rename documented in the MudBlazor v9.0.0 migration guide (https://github.com/MudBlazor/MudBlazor/issues/12666) — verify any non-obvious fix against MudBlazor's actual source at the `v9.6.0` git tag before applying it. If a build error doesn't match anything in the guide, stop and report it rather than inventing a fix.

- [ ] **Step 12: Commit**

```bash
git add src/frontend/BlazorTeste/BlazorTeste.csproj \
        src/frontend/BlazorTeste/Components/Pages/Negociacao/Negociacoes.razor \
        src/frontend/BlazorTeste/Components/Pages/Configuracoes/Configuracoes.razor \
        src/frontend/BlazorTeste/Components/Pages/Juridico/Juridico.razor \
        src/frontend/BlazorTeste/Components/Pages/GuiaSindical/GuiaSindicais.razor \
        src/frontend/BlazorTeste/Components/Pages/Cobranca/Cobrancas.razor \
        src/frontend/BlazorTeste/Components/Pages/Contribuintes/Contribuintes.razor \
        src/frontend/BlazorTeste/Components/Pages/Baixa/BaixaCobranca.razor \
        src/frontend/BlazorTeste/Components/Pages/Financeiro/Financeiro.razor \
        src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor
git commit -m "chore(frontend): bump MudBlazor 8.15.0 -> 9.6.0"
```

(If Step 11 required fixes in additional files, include those in the `git add` as well.)

---

## Task 4: Manual smoke test

**Files:** none (verification only, per the design spec's chosen verification strategy — build-driven per major, single manual pass at the end).

- [ ] **Step 1: Start backend and frontend**

Terminal 1 (from `src/backend`): `dotnet run --project BlazorTeste.Api`
Terminal 2 (from `src/frontend/BlazorTeste`): `dotnet run`

- [ ] **Step 2: Login**

Open the frontend in a browser, log in with `admin@dpi.com.br` / `Senha@123` (or any other seeded user). Expected: login form renders correctly (fields, button), submits, redirects to the dashboard.

- [ ] **Step 3: Dashboard**

Expected: dashboard cards, chart, and table sections render without visual breakage (this exercises `MudCard`, `MudChart`, `MudTable`, `MudChip`, `MudGrid` — the highest-volume components in the app).

- [ ] **Step 4: A page with `MudTabs`**

Navigate to Contribuintes (`/contribuintes`). Expected: tab bar renders, clicking between tabs switches content, tab panel padding (`TabPanelsClass="pt-3"` from Task 3) still applies visually (no content jammed against the tab bar).

- [ ] **Step 5: A dialog**

On the Contribuintes page (or Cobranças/Financeiro), open the "Novo Contribuinte" (or equivalent "Novo X") dialog. Expected: dialog opens, form fields render, clicking Cancel closes it, filling required fields and clicking Save/Confirm closes it and the underlying page reflects the change (or at minimum doesn't error).

- [ ] **Step 6: A menu**

Click the user avatar menu in the top-right of `MainLayout` (Perfil/Sair). Expected: menu opens on click, closes on an outside click or after selecting an item — this is the component most likely to show the v9 `Modal` default change (`true`→`false`). If the menu now stays open when it shouldn't, or closes unexpectedly, that is the concrete regression the design spec anticipated; note it precisely (component, page, what happened vs. expected) rather than guessing a global fix.

- [ ] **Step 7: A select or date picker**

On the Cobranças or Negociações page, open a filter's `MudSelect` or `MudDatePicker`. Expected: dropdown/calendar opens positioned near the field (not flipped to an unexpected screen position — v9 changed default flip behavior to `FlipAlways`), selecting a value closes it and updates the field.

- [ ] **Step 8: Report findings**

If Steps 2-7 all pass with no visual/functional regression, the migration is done — no further code changes needed. If any step revealed a regression, note the exact component, page, and behavior (expected vs. actual) so it can be fixed as a follow-up, scoped to that specific instance rather than a blanket configuration change (per the design spec's decision not to override `Modal`/`PopoverOptions` defaults preventively).

No commit for this task — it's verification only.
