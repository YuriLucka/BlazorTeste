# NovoContribuinteDialog Validation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make `NovoContribuinteDialog.razor`'s "Salvar" button actually enforce the 6 fields already marked required (CNPJ, Razão Social, CEP, Logradouro, Bairro, Cidade) instead of only checking Razão Social.

**Architecture:** Wrap the dialog's `MudTabs` in a `MudForm`, add a per-field `Validation` func (non-empty check) to the 6 required fields, and set `KeepPanelsAlive="true"` on `MudTabs` so fields in inactive tabs stay registered with the form. `Submit()` becomes async, calls `_form.Validate()`, and either blocks (snackbar + jump to the first tab with an error) or closes the dialog as before.

**Tech Stack:** Blazor, MudBlazor 6.x (`MudForm`, `MudTabs.KeepPanelsAlive`).

## Global Constraints

- Only validate "not empty" on the 6 required fields — no CNPJ check-digit validation, no CEP regex, no email format checks on Contatos/Sócios.
- Do not add a real API call — the dialog keeps closing via `MudDialog.Close(DialogResult.Ok(true))`; no create-Contribuinte endpoint exists yet.
- Do not touch Contatos/Sócios collection validation — they stay optional, unchanged.
- No automated test project exists for the frontend (no bUnit project in `tests/`) — verification is manual via browser, not a unit test.

---

### Task 1: Wrap dialog in MudForm and enforce the 6 required fields

**Files:**
- Modify: `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor`

**Interfaces:**
- Produces: `private MudForm _form = null!;` (field ref used by `Submit()`), `private async Task Submit()` (replaces the existing `private void Submit()`, same `OnClick="Submit"` binding — Blazor accepts `Task`-returning event handlers with identical syntax), `private int PrimeiraAbaComErro()`, `private static string? ValidarObrigatorio(string valor, string mensagem)`.

- [ ] **Step 1: Wrap `MudTabs` in `MudForm` and enable `KeepPanelsAlive`**

In `src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor`, find this line (currently line 11):

```razor
        <MudTabs Elevation="0" @bind-ActivePanelIndex="_tab" PanelClass="pt-4">
```

Replace it with (add the `MudForm` wrapper opening tag before it, and add `KeepPanelsAlive="true"` to `MudTabs`):

```razor
        <MudForm @ref="_form">
        <MudTabs Elevation="0" @bind-ActivePanelIndex="_tab" PanelClass="pt-4" KeepPanelsAlive="true">
```

Then find the closing `</MudTabs>` tag (currently line 221, immediately followed by `</DialogContent>` on line 222):

```razor
        </MudTabs>
    </DialogContent>
```

Replace it with (add the matching `MudForm` closing tag):

```razor
        </MudTabs>
        </MudForm>
    </DialogContent>
```

- [ ] **Step 2: Add `Validation` to the CNPJ field**

Find (currently lines 17-21):

```razor
                        <MudTextField @bind-Value="_cnpj" Label="CNPJ *" Variant="Variant.Outlined"
                                      Mask="@(new PatternMask("00.000.000/0000-00"))"
                                      Placeholder="00.000.000/0000-00"
                                      Adornment="Adornment.Start" AdornmentIcon="@Icons.Material.Filled.Badge"
                                      Required="true" RequiredError="CNPJ é obrigatório" />
```

Replace with:

```razor
                        <MudTextField @bind-Value="_cnpj" Label="CNPJ *" Variant="Variant.Outlined"
                                      Mask="@(new PatternMask("00.000.000/0000-00"))"
                                      Placeholder="00.000.000/0000-00"
                                      Adornment="Adornment.Start" AdornmentIcon="@Icons.Material.Filled.Badge"
                                      Required="true" RequiredError="CNPJ é obrigatório"
                                      Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "CNPJ é obrigatório.")))" />
```

- [ ] **Step 3: Add `Validation` to the Razão Social field**

Find (currently lines 30-32):

```razor
                        <MudTextField @bind-Value="_razaoSocial" Label="Razão Social *" Variant="Variant.Outlined"
                                      MaxLength="60" Required="true" RequiredError="Razão Social é obrigatória"
                                      Counter="60" />
```

Replace with:

```razor
                        <MudTextField @bind-Value="_razaoSocial" Label="Razão Social *" Variant="Variant.Outlined"
                                      MaxLength="60" Required="true" RequiredError="Razão Social é obrigatória"
                                      Counter="60"
                                      Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "Razão Social é obrigatória.")))" />
```

- [ ] **Step 4: Add `Validation` to the CEP field**

Find (currently lines 80-82):

```razor
                        <MudTextField @bind-Value="_cep" Label="CEP *" Variant="Variant.Outlined"
                                      Mask="@(new PatternMask("00000-000"))"
                                      Placeholder="00000-000" Required="true" />
```

Replace with:

```razor
                        <MudTextField @bind-Value="_cep" Label="CEP *" Variant="Variant.Outlined"
                                      Mask="@(new PatternMask("00000-000"))"
                                      Placeholder="00000-000" Required="true"
                                      Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "CEP é obrigatório.")))" />
```

- [ ] **Step 5: Add `Validation` to the Logradouro field**

Find (currently lines 85-86):

```razor
                        <MudTextField @bind-Value="_logradouro" Label="Logradouro (Rua/Av) *" Variant="Variant.Outlined"
                                      MaxLength="60" Required="true" />
```

Replace with:

```razor
                        <MudTextField @bind-Value="_logradouro" Label="Logradouro (Rua/Av) *" Variant="Variant.Outlined"
                                      MaxLength="60" Required="true"
                                      Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "Logradouro é obrigatório.")))" />
```

- [ ] **Step 6: Add `Validation` to the Bairro field**

Find (currently line 95):

```razor
                        <MudTextField @bind-Value="_bairro" Label="Bairro *" Variant="Variant.Outlined" MaxLength="60" Required="true" />
```

Replace with:

```razor
                        <MudTextField @bind-Value="_bairro" Label="Bairro *" Variant="Variant.Outlined" MaxLength="60" Required="true"
                                      Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "Bairro é obrigatório.")))" />
```

- [ ] **Step 7: Add `Validation` to the Cidade field**

Find (currently line 98):

```razor
                        <MudTextField @bind-Value="_cidade" Label="Cidade *" Variant="Variant.Outlined" MaxLength="60" Required="true" />
```

Replace with:

```razor
                        <MudTextField @bind-Value="_cidade" Label="Cidade *" Variant="Variant.Outlined" MaxLength="60" Required="true"
                                      Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "Cidade é obrigatória.")))" />
```

- [ ] **Step 8: Add the `_form` field and rewrite `Submit()`**

In the `@code` block, find (currently lines 249-253):

```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public bool IsEdicao { get; set; }

    private int _tab = 0;
```

Replace with (adds the `_form` field):

```csharp
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public bool IsEdicao { get; set; }

    private MudForm _form = null!;
    private int _tab = 0;
```

Then find the existing `Submit()` method (currently lines 288-299):

```csharp
    private void Submit()
    {
        if (string.IsNullOrWhiteSpace(_razaoSocial))
        {
            Snackbar.Add("Razão Social é obrigatória.", Severity.Warning);
            _tab = 0;
            return;
        }
        MudDialog.Close(DialogResult.Ok(true));
    }
```

Replace it with:

```csharp
    private async Task Submit()
    {
        await _form.Validate();
        if (!_form.IsValid)
        {
            Snackbar.Add("Corrija os campos obrigatórios.", Severity.Warning);
            _tab = PrimeiraAbaComErro();
            return;
        }
        MudDialog.Close(DialogResult.Ok(true));
    }

    private int PrimeiraAbaComErro()
    {
        if (string.IsNullOrWhiteSpace(_cnpj) || string.IsNullOrWhiteSpace(_razaoSocial))
            return 0;
        if (string.IsNullOrWhiteSpace(_cep) || string.IsNullOrWhiteSpace(_logradouro) ||
            string.IsNullOrWhiteSpace(_bairro) || string.IsNullOrWhiteSpace(_cidade))
            return 1;
        return 0;
    }

    private static string? ValidarObrigatorio(string valor, string mensagem) =>
        string.IsNullOrWhiteSpace(valor) ? mensagem : null;
```

- [ ] **Step 9: Build to verify it compiles**

Run: `dotnet build src/frontend/BlazorTeste/BlazorTeste.csproj`
Expected: `Build succeeded`, 0 errors. (If Visual Studio has the frontend project open/running and locks output DLLs, a build failure due to file locks is an environment issue, not a code defect — report it as such rather than treating it as a compile error in the new code.)

- [ ] **Step 10: Commit**

```bash
git add src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor
git commit -m "feat(frontend): enforce required fields in NovoContribuinteDialog via MudForm"
```

---

### Task 2: Manual browser verification

**Files:** none (verification only, no code changes)

- [ ] **Step 1: Confirm the running frontend picked up the change**

The frontend at `https://localhost:7130` may already be running under Visual Studio (check with the user if unsure, or check for a process listening on port 7130/5233). Blazor Server/WASM dev builds typically hot-reload `.razor` changes; if the browser doesn't reflect the new behavior after a hard refresh, ask the user to rebuild/restart the frontend in Visual Studio before continuing — do not attempt to kill or restart their IDE-managed process yourself.

- [ ] **Step 2: Open the dialog and submit empty**

Navigate to `https://localhost:7130/contribuintes`, click "NOVO CONTRIBUINTE", click "PRÓXIMO" three times to reach the Sócios tab (all fields still empty), then click "SALVAR".

Expected: dialog does NOT close. A warning snackbar reads "Corrija os campos obrigatórios." and the dialog jumps back to the "Dados Gerais" tab (tab 0), since CNPJ and Razão Social are both empty.

- [ ] **Step 3: Fill only Dados Gerais, leave Endereço empty, submit**

Fill CNPJ (e.g. `12.345.678/0001-90`) and Razão Social (e.g. `Empresa Teste LTDA`) on the Dados Gerais tab. Click "PRÓXIMO" to reach Endereço (leave CEP/Logradouro/Bairro/Cidade empty), then "PRÓXIMO" twice more to Sócios, then "SALVAR".

Expected: dialog does NOT close. Snackbar "Corrija os campos obrigatórios." appears, and the dialog jumps to the "Endereço" tab (tab 1), since CEP/Logradouro/Bairro/Cidade are empty while Dados Gerais is now valid.

- [ ] **Step 4: Fill all 6 required fields, submit**

Also fill CEP (e.g. `01310-100`), Logradouro (e.g. `Av Paulista`), Bairro (e.g. `Bela Vista`), Cidade (e.g. `São Paulo`). Click "SALVAR" from the Sócios tab.

Expected: dialog closes (no snackbar, no validation block) — matches the pre-existing "closes on valid Razão Social" behavior, now gated on all 6 fields instead of just one.

No commit for this task — it's verification only.
