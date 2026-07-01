# Validação real no NovoContribuinteDialog — design

## Contexto

`NovoContribuinteDialog.razor` (`src/frontend/BlazorTeste/Components/Dialogs/Contribuinte/NovoContribuinteDialog.razor`) é um form de 4 abas (Dados Gerais, Endereço, Contatos, Sócios) usado tanto para criação quanto edição de Contribuinte. Vários campos são marcados visualmente como obrigatórios (`**` no label, atributo `Required="true"` do MudBlazor), mas isso é puramente cosmético: os campos não estão dentro de um `MudForm`, então nada é de fato validado.

O método `Submit()` atual só checa `string.IsNullOrWhiteSpace(_razaoSocial)`. CNPJ (obrigatório) e a aba inteira de Endereço (CEP, Logradouro, Bairro, Cidade — todos obrigatórios) passam sem nenhuma checagem, inclusive ao trocar de aba (`_tab++`/`_tab--` também não valida nada).

Confirmado via teste manual no navegador: dá pra avançar as 4 abas com tudo vazio e só toma um aviso de Razão Social ao clicar Salvar; CNPJ e Endereço ficam vazios sem aviso algum.

Nenhum outro componente do projeto usa `MudForm` hoje (grep confirmou zero ocorrências em `src/frontend`) — este será o primeiro uso desse padrão.

## Objetivo

Fazer o dialog validar de verdade os 6 campos hoje marcados como obrigatórios antes de fechar: `CNPJ`, `Razão Social`, `CEP`, `Logradouro`, `Bairro`, `Cidade`. Nada além disso.

## Abordagem

Envolver o conteúdo do `DialogContent` (o `MudTabs` inteiro) em `<MudForm @ref="_form">`. Cada um dos 6 campos obrigatórios ganha `Validation` apontando para uma função local que retorna `null` (válido) ou uma string de erro quando o valor está vazio/branco — sem checar formato (sem dígito verificador de CNPJ, sem regex de CEP).

### Gotcha: MudTabs desmonta painéis inativos

Por padrão, `MudTabs` só mantém o painel da aba ativa no DOM — trocar de aba destrói o painel anterior. Isso significa que campos das abas 1–3 (Endereço, Contatos, Sócios) se desregistram do `MudForm` quando a aba muda, e como o botão "Salvar" só existe na última aba (Sócios), no momento do clique o `MudForm` só enxergaria os campos da aba 4 — CNPJ e Endereço já teriam se desregistrado e passariam despercebidos.

Fix: adicionar `KeepPanelsAlive="true"` ao `MudTabs`. Esse parâmetro existe no MudBlazor 6.x (versão já instalada no projeto, `BlazorTeste.csproj:14`) e mantém todos os painéis montados no DOM (escondidos via CSS ao invés de destruídos), garantindo que o `MudForm` sempre veja todos os campos independente da aba ativa.

### Submit() reescrito

`private void Submit()` vira `private async Task Submit()`:

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
```

`OnClick="Submit"` no `MudButton` de Salvar continua funcionando sem mudança — Blazor aceita event handlers `async Task` na mesma sintaxe.

### Funções de validação por campo

Uma função privada reutilizada em todos os 6 campos (mesma regra "não vazio"), parametrizada pela mensagem:

```csharp
private static string? ValidarObrigatorio(string valor, string mensagem) =>
    string.IsNullOrWhiteSpace(valor) ? mensagem : null;
```

Uso em cada `MudTextField`, ex.:

```razor
<MudTextField @bind-Value="_cnpj" Label="CNPJ *" ...
              Validation="@(new Func<string, string?>(v => ValidarObrigatorio(v, "CNPJ é obrigatório.")))" />
```

## Fora de escopo

- Validação de formato: dígito verificador de CNPJ, regex de CEP, formato de e-mail nos Contatos/Sócios (campos não marcados obrigatórios).
- Chamada real de API: o dialog continua fechando local via `MudDialog.Close(DialogResult.Ok(true))` — não existe endpoint de criar Contribuinte no backend ainda, isso é trabalho futuro separado.
- Validação de Contatos/Sócios (coleções): permanecem opcionais, sem mudança.
