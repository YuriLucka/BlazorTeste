# Migração MudBlazor 6.21.0 → 9.6.0 — design

## Contexto

O frontend (`src/frontend/BlazorTeste`, Blazor WebAssembly, .NET 10) referencia `MudBlazor` `Version="6.*"` (resolvido hoje em `6.21.0`). Segundo a matriz oficial de suporte (mudblazor.com), a série 6.x teve suporte encerrado em jan/2025 e só a série 9.x tem suporte pleno pra .NET 10. Última patch estável de cada major, confirmado via NuGet: `7.16.0`, `8.15.0`, `9.6.0`.

Levantamento do uso real no projeto (grep, não estimativa): 31 arquivos `.razor`, todos usam MudBlazor, 47 tipos de componente distintos. Maior volume: `MudText` (302x), `MudItem` (204x), `MudButton` (73x), `MudTextField` (61x), `MudTabPanel` (44x), `MudChip` (39x), `MudTable` (23x), `MudSelect` (16x), `MudDialog`/`MudDatePicker` (11x cada), `MudDataGrid` (1x). Único host WASM puro, sem SSR/Server — não afetado pela remoção de suporte a static rendering no MudBlazor 8/9.

## Objetivo

Atualizar o projeto pra MudBlazor 9.6.0, migrando incrementalmente por major (6→7→8→9) pra usar o compilador + analyzer do MudBlazor como guia a cada passo, em vez de acumular 3 rodadas de breaking changes antes do primeiro build.

## Decisões

- **Migração incremental, não direta 6→9.** Cada major sobe isolado, com `dotnet build` limpo (0 erros, 0 warnings do analyzer de atributos órfãos que o MudBlazor embarca desde v7) antes de avançar pro próximo. Attribute splatting no Razor faz com que parâmetros renomeados não gerem erro de compilação — só o analyzer pega isso.
- **Verificação:** build limpo a cada major (7, 8, 9) + smoke test manual leve no browser só depois do v9 final (login, dashboard, uma tabela, um diálogo, um menu). Não há suite de teste automatizado no frontend (limitação já documentada em trabalhos anteriores desta sessão).
- **Default `Modal=false` em popovers (mudança v9, afeta `MudMenu`/`MudSelect`/`MudAutocomplete`/pickers)** — aceito sem override explícito. Se o smoke test final revelar regressão visual, corrige pontualmente componente por componente, não em bloco preventivo.
- **Versões pinadas exatas** (`7.16.0`, `8.15.0`, `9.6.0`), não faixas `7.*`/`8.*`/`9.*` — states intermediários de uma migração incremental não devem ficar sujeitos a drift de patch.

## Levantamento de pontos de quebra confirmados

Grep direto no código (não só leitura do guia oficial) — cada item abaixo é usage real, com arquivo:linha.

### v7 (`Icons.Filled`→`Icons.Material.Filled`, `MudPopoverProvider`, `Palette`→`PaletteLight`, `grey`→`gray`, `DisableX`→`X` invertido, `MudChipSet`/`MudChip` genéricos)

- **Ícones:** já usam `Icons.Material.Filled.*` em todo o projeto — **nenhuma mudança necessária**.
- **`<MudPopoverProvider />`:** ausente em `src/frontend/BlazorTeste/Components/Layout/MainLayout.razor` (hoje só tem `<MudThemeProvider/>`, `<MudDialogProvider/>`, `<MudSnackbarProvider/>`) — **adicionar**, obrigatório desde v7.
- **`MudTheme.Palette`:** `MainLayout.razor:108-118` define `Palette = new PaletteLight {...}` e `PaletteDark = new PaletteDark {...}`. Propriedade `Palette` foi renomeada pra `PaletteLight` em v7 (fica estável até v9, onde só o *tipo* subjacente muda de `PaletteLight`/`PaletteDark` pra `Palette` base — instanciar as classes derivadas continua funcionando) — **renomear a propriedade**.
- **CSS var `grey`→`gray`:** `Components/Pages/Negociacao/Negociacoes.razor:145`, `style="...background:var(--mud-palette-background-grey)"` — **renomear pra `--mud-palette-background-gray`**.
- **`DisableX` props:** grep não encontrou nenhum uso (`DisableRipple`, `DisableGutters`, `DisablePadding`, `DisableElevation`, `DisableUnderLine`, `DisableRowsPerPage`, `DisableToolbar`, `DisableSidePadding`, `DisableOverlay`, `DisableSliderAnimation`, `DisableModifiers`, `DisableBackdropClick`, `DisableBorders`) — **nenhuma mudança necessária**.
- **`MudChip`/`MudChipSet` genéricos:** 39 usos de `<MudChip` em 16 arquivos, **todos já têm `T="string"`**. Nenhum `<MudChipSet` no projeto — chips são só exibição (badges de status), sem seleção. Nenhum uso de `MultiSelection`, `Mandatory`, `SelectedChip`, `SelectedChips`, `Avatar`, `AvatarClass` nos chips — **risco baixo, provavelmente nenhuma mudança de código necessária além do que o build apontar**.
- **`MudTable.ServerData`:** grep não encontrou uso — todas as tabelas usam `Items=` com coleção já carregada — **nenhuma mudança necessária**.

### v8 (`MudDialogInstance`→`IMudDialogInstance`, `.Close()`→`.CloseAsync()`, `DialogOptions` imutável)

9 arquivos de diálogo, todos com o mesmo padrão — campo `[CascadingParameter] private MudDialogInstance MudDialog` e método síncrono chamando `MudDialog.Close(...)`:

- `Components/Shared/ConfirmDialog.razor:29,37`
- `Components/Dialogs/Cobranca/NovaCobrancaDialog.razor:74,105`
- `Components/Dialogs/Contribuinte/InativarContribuinteDialog.razor:25,28`
- `Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor:258,308`
- `Components/Dialogs/Evento/NovoEventoDialog.razor:196,228`
- `Components/Dialogs/Financeiro/NovoLancamentoDialog.razor:148,194`
- `Components/Dialogs/GuiaSindical/EmitirGuiaSindicalDialog.razor:204,240`
- `Components/Dialogs/Juridico/NovoProcessoDialog.razor:113,146`
- `Components/Dialogs/Negociacao/NovaNegociacaoDialog.razor:247,299`

Em cada um: trocar `MudDialogInstance` por `IMudDialogInstance`, e o método que chama `.Close(...)` precisa virar `async Task` chamando `await MudDialog.CloseAsync(...)`.

Do lado chamador (`DialogService.ShowAsync<T>(...)`) — já é assíncrono em todos os 7 pontos de chamada (`Cobrancas.razor:119`, `Eventos.razor:257`, `Financeiro.razor:130`, `Juridico.razor:132`, `GuiaSindicais.razor:239`, `Negociacoes.razor:316`, `Contribuintes.razor:250`) — **nenhuma mudança necessária no lado chamador**.

`DialogOptions` virar record imutável (v8) só importa se algum código tenta mutar options depois de criadas — não é o padrão usado aqui (options são construídas inline no `ShowAsync`), **sem impacto esperado**, mas o build vai confirmar.

### v9 (`PanelClass`→`TabPanelsClass`, `Palette` tipo base, `Modal` default)

- **`<MudTabs PanelClass="...">`:** 9 usos, todos no elemento `<MudTabs>` (não em `<MudTabPanel>`), todos definindo classe de padding (`pt-2`/`pt-3`/`pt-4`):
  - `Components/Pages/Negociacao/Negociacoes.razor:80`
  - `Components/Pages/Configuracoes/Configuracoes.razor:22`
  - `Components/Pages/Juridico/Juridico.razor:19`
  - `Components/Pages/GuiaSindical/GuiaSindicais.razor:80`
  - `Components/Pages/Cobranca/Cobrancas.razor:21`
  - `Components/Pages/Contribuintes/Contribuintes.razor:73`
  - `Components/Pages/Baixa/BaixaCobranca.razor:64`
  - `Components/Pages/Financeiro/Financeiro.razor:19`
  - `Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor:12`

  Renomear `PanelClass=` pra `TabPanelsClass=` em todos os 9.
- **`Modal` default `true`→`false`** em componentes baseados em popover (`MudMenu`, `MudSelect`, `MudAutocomplete`, pickers) — sem override, conforme decisão acima.

## Plano de execução

1. **v6→v7:** bump `MudBlazor` pra `7.16.0` no `.csproj`, `dotnet build`, corrigir os 3 itens confirmados de v7 acima (popover provider, palette rename, grey→gray CSS var) + qualquer coisa que o analyzer/compilador apontar além do que o grep já cobriu, commit.
2. **v7→v8:** bump pra `8.15.0`, `dotnet build`, corrigir os 9 diálogos (`MudDialogInstance`→`IMudDialogInstance`, `.Close()`→`await .CloseAsync()`) + qualquer coisa nova que o build apontar, commit.
3. **v8→v9:** bump pra `9.6.0`, `dotnet build`, corrigir os 9 `PanelClass`→`TabPanelsClass` + qualquer coisa nova, commit.
4. **Smoke test manual** no browser (login, dashboard, uma tabela, um diálogo — ex. "Novo Contribuinte", um menu) pra pegar regressões visuais silenciosas (`Modal` default, popover flip behavior) que não geram erro de build.

## Fora de escopo

- Adotar `MudDataGrid` no lugar de `MudTable` em qualquer lugar — só migração de versão, sem troca de componente.
- Configurar `PopoverOptions`/`SnackbarConfiguration` customizados em `AddMudServices` — usa os defaults do v9 (incluindo `Modal=false`, `FlipAlways`) a menos que o smoke test revele necessidade concreta de mudar.
- Migrar `MudChart`, `MudDataGrid`, `MudStepper`, `MudFileUpload`, `MudChat` — não usados neste projeto ou usados de forma trivial sem tocar nas APIs que mudaram nesses componentes.
- Testes automatizados de frontend — segue sem suite (limitação já documentada em trabalhos anteriores desta sessão).
