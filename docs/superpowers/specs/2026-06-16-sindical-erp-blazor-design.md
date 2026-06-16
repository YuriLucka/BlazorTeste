# Design — Sistema ERP Sindical (Blazor WASM MVP)

> Spec aprovado em 2026-06-16. Baseado em `docs/dominios-e-plano-inicial-projeto.md` e `docs/reuniao-projeto-sistema-sindicato.md`.

## Objetivo

MVP navegável em Blazor WebAssembly para exploração da plataforma. Todas as telas com dados mock realistas. Foco em arquitetura de componentes, navegação e visual moderno — não em lógica de negócio completa.

## Decisões de tecnologia

| Decisão | Escolha | Motivo |
|---------|---------|--------|
| Runtime | Blazor WebAssembly standalone (.NET 10) | Client-side puro, sem servidor Blazor |
| UI | MudBlazor 8.x | Material Design 3, melhor ecossistema Blazor |
| Dados | Serviços in-memory (Singleton) | Demo/exploração, sem backend |
| Estado | DI Singleton services | Dados persistem na sessão do browser |

## Arquitetura

### Estrutura de projeto

```
BlazorTeste/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor        # shell: sidebar + topbar
│   │   └── NavMenu.razor           # links por módulo
│   ├── Pages/
│   │   ├── Dashboard/
│   │   │   └── Dashboard.razor
│   │   ├── Contribuintes/
│   │   │   ├── Contribuintes.razor
│   │   │   └── ContribuinteDetalhe.razor
│   │   ├── Cobranca/
│   │   │   └── Cobrancas.razor
│   │   ├── Juridico/
│   │   │   └── Juridico.razor
│   │   ├── Financeiro/
│   │   │   └── Financeiro.razor
│   │   ├── Mailing/
│   │   │   └── Mailing.razor
│   │   ├── Entidades/
│   │   │   └── Entidades.razor
│   │   └── Usuarios/
│   │       └── Usuarios.razor
│   └── Shared/
│       └── (componentes reutilizáveis futuros)
├── Services/
│   ├── ContribuinteService.cs
│   ├── CobrancaService.cs
│   ├── JuridicoService.cs
│   ├── FinanceiroService.cs
│   ├── MailingService.cs
│   ├── EntidadeService.cs
│   └── UsuarioService.cs
├── Models/
│   ├── Contribuinte.cs
│   ├── Cobranca.cs
│   ├── Processo.cs
│   ├── LancamentoFinanceiro.cs
│   ├── Campanha.cs
│   ├── Entidade.cs
│   └── Usuario.cs
└── wwwroot/
```

### Conversão Server → WASM

`Program.cs` muda de `AddInteractiveServerComponents()` para `AddInteractiveWebAssemblyComponents()` (ou projeto standalone WASM puro com `WebAssemblyHostBuilder`).

`App.razor` remove `@rendermode InteractiveServer`, páginas usam `@rendermode InteractiveWebAssembly`.

## Layout & Navegação

### Shell principal

- **Sidebar esquerda**: colapsável, ícones + labels, agrupa módulos por categoria
- **Topbar**: chip da entidade ativa (clicável → dialog troca entidade), avatar usuário, toggle dark/light mode, breadcrumb
- **Responsive**: sidebar vira `MudDrawer` em mobile

### Módulos

| Módulo | Rota | Ícone |
|--------|------|-------|
| Dashboard | `/` | `Dashboard` |
| Contribuintes | `/contribuintes` | `Business` |
| Cobranças | `/cobrancas` | `RequestQuote` |
| Jurídico | `/juridico` | `Gavel` |
| Financeiro | `/financeiro` | `AccountBalance` |
| Mailing | `/mailing` | `Email` |
| Entidades | `/entidades` | `AccountTree` |
| Usuários | `/usuarios` | `People` |

### Tema MudBlazor

```csharp
new MudTheme {
    PaletteLight = new PaletteLight {
        Primary = "#1565C0",
        Secondary = "#00897B",
        Background = "#F5F7FA",
        Surface = "#FFFFFF",
        AppbarBackground = "#1565C0"
    },
    PaletteDark = new PaletteDark {
        Primary = "#5C9CE6",
        Secondary = "#4DB6A6",
        Background = "#1A1A2E",
        Surface = "#16213E"
    },
    LayoutProperties = new LayoutProperties {
        DefaultBorderRadius = "12px"
    }
}
```

## Telas por módulo

### Dashboard `/`

Componentes MudBlazor usados: `MudGrid`, `MudCard`, `MudChart` (BarChart), `MudTable`, `MudChip`

- 4 KPI cards: Total contribuintes, Cobranças em aberto (R$), Arrecadado no mês, Processos ativos
- Gráfico de barras: arrecadação últimos 6 meses
- Tabela: últimas 5 cobranças emitidas (contribuinte, tipo, valor, vencimento, status)
- Lista: próximas 3 audiências (processo, advogado, data)

### Contribuintes `/contribuintes`

Componentes: `MudDataGrid`, `MudTextField` (busca), `MudDrawer` (detalhe), `MudTabs`

- DataGrid paginada: CNPJ, razão social, CNAE, situação, data cadastro
- Busca por CNPJ ou razão social
- Clique na linha abre drawer lateral com abas:
  - **Dados**: razão social, CNPJ, capital social, nº funcionários, regime tributário
  - **Endereços**: lista de endereços com tipo (Estabelecimento, Cobrança, Jurídico)
  - **Contatos**: e-mails e telefones separados
  - **Sócios**: contribuintes vinculados como sócio com matrícula
  - **Histórico**: tabela mensal de capital social / nº funcionários

### Cobranças `/cobrancas`

Componentes: `MudDataGrid`, `MudChip` (status), `MudDialog` (detalhe boleto), `MudTabs`

- Tabs: Pendentes | Pagas | Vencidas | Todas
- DataGrid: contribuinte, tipo cobrança, vencimento, valor, multa/juros, status
- Chips coloridos: Verde=Pago, Vermelho=Vencido, Azul=Pendente, Cinza=Cancelado
- Clique abre dialog com detalhe: linha digitável mock, histórico, botão "Emitir 2ª via"

### Jurídico `/juridico`

Componentes: `MudTabs`, `MudDataGrid`, `MudDialog` (form), `MudForm`

- Tabs: Processos | Advogados | Audiências
- Processos: número, tipo, vara, situação, advogado responsável
- Advogados: nome, OAB, e-mail, processos ativos
- Audiências: data, processo, advogado, tipo (reordenável por data)
- Botão "Novo" em cada aba abre dialog com form validado (MudForm)

### Financeiro `/financeiro`

Componentes: `MudTabs`, `MudDataGrid`, `MudTable`

- Tabs: Lançamentos | Fluxo de Caixa | Fornecedores
- Lançamentos: data, categoria, fornecedor, valor, tipo (entrada/saída), conta bancária
- Fluxo de caixa: tabela mês a mês (entradas previstas, entradas realizadas, saídas, saldo)
- Fornecedores: nome, CNPJ, categoria, contato

### Mailing `/mailing`

Componentes: `MudCard` (por campanha), `MudChip` (status), `MudGrid`

- Grid de cards por campanha: assunto, destinatários, data, status
- Status chips: Rascunho (cinza), Agendada (azul), Enviada (verde), Erro (vermelho)
- Botão "Nova campanha" abre dialog com form

### Entidades `/entidades`

Componentes: `MudCard`, `MudGrid`, `MudDrawer`

- Grid de cards: logo simulado (avatar com iniciais), nome, CNPJ, cidade sede, total contribuintes
- Clique abre drawer: dados da entidade + abas Base de Abrangência (CNAEs, cidades)

### Usuários `/usuarios`

Componentes: `MudDataGrid`, `MudAvatar`, `MudChip`

- DataGrid: avatar (iniciais), nome, e-mail, entidades vinculadas (chips), último acesso
- Chips de entidade clicáveis mostram permissões por entidade

## Dados mock

Cada serviço é um Singleton com listas em memória inicializadas no construtor:

- **5 entidades** (sindicatos com nomes realistas de SP)
- **50 contribuintes** com CNPJs válidos formatados, distribuídos por entidade
- **80 cobranças** (mix de tipos, status e vencimentos)
- **15 processos jurídicos**, **8 advogados**, **20 audiências**
- **30 lançamentos financeiros** (últimos 6 meses)
- **10 campanhas** de mailing
- **6 usuários** com permissões variadas por entidade

## Fora do escopo deste MVP

- Autenticação real (login é simulado)
- Geração de boleto real
- Envio de e-mail real
- Persistência (refresh limpa dados)
- Portal do Contribuinte (tela externa)
- Motor de regras de cobrança configurável
