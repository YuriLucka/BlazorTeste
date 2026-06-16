# Domínios do Sistema e Plano Inicial — Novo Sistema para Entidades Sindicais

> Documento de trabalho, em construção. Combina o levantamento detalhado feito na reunião de transcrição (`reuniao-projeto-sistema-sindicato.md`) com o contexto trazido da reunião com o Anderson. A ideia é ir complementando conforme novas anotações forem chegando.

## Contexto atualizado

- Projeto com previsão de **14 meses** até estar pronto.
- O contrato com o cliente já foi fechado: é um **projeto novo**, mas reaproveitando os dados do sistema atual — ou seja, vai exigir migração completa da base.
- As entidades (sindicatos) serão, de certa forma, **retrabalhadas** na nova estrutura — não é uma migração 1:1, é preciso repensar o modelo de dados.
- O conceito geral do sistema permanece o mesmo do legado, mas com arquitetura mais moderna, melhor organizada e mais consistente (alinhado ao que já tinha sido discutido no levantamento original).
- Nomenclatura: o sistema antigo chama os tenants de "sindicato"; como ele também atende federações e confederações, o termo mais adequado é **entidade** (já estava sugerido no levantamento original, trocando `CD_SINDICATO` por `CD_ENTIDADE`).
- Back-end confirmado: a plataforma antiga foi feita em **Web Forms**, a nova será em **ASP.NET Core**. A abordagem de front-end ainda não foi decidida (ver "Decisões em aberto").
- Confirmado com o Anderson: o sistema antigo **não tem integração com APIs externas** — a parte bancária, por exemplo, depende de bibliotecas internas (DLL), não de uma API de terceiros.
- O sistema antigo é usado só internamente (pela equipe do sindicato). No novo, vai haver **duas plataformas**: o sistema interno (equivalente ao atual) e o Portal do Contribuinte, de acesso externo.
- Tipos de usuário: hoje não existe um perfil de "administrador" definido — qualquer um com acesso à tela de usuários pode fazer qualquer coisa nela. No sistema novo, a ideia é ter algum perfil responsável pela administração da plataforma.

## Anotações importantes (transversais)

Pontos que não são específicos de um único domínio, mas afetam o sistema como um todo:

- **Logs e rastreabilidade.** O sistema precisa manter logs bem rastreáveis, com histórico de mudanças/alterações — não só na parte de cobrança (onde isso já tinha sido identificado como necessário), mas como requisito do sistema inteiro.
- **Endereços do contribuinte.** Assim como na plataforma antiga, o contribuinte poderá ter vários endereços, mas obrigatoriamente precisa ter um endereço do estabelecimento em si; os demais (cobrança, jurídico etc.) seguem o que for definido para cada caso. O importante é nunca perder de vista qual é o endereço real do estabelecimento.
- **Permissões por entidade.** Reforçando o que já estava no levantamento original: as permissões do usuário são por entidade — um usuário pode estar vinculado a mais de uma entidade, com permissões diferentes em cada uma.
- **Regras de cobrança configuráveis.** Hoje o Anderson precisa alterar as regras de cobrança direto no código quando algo muda. A meta do sistema novo é dar esse controle aos próprios usuários, com alguma estrutura que dê conta da variedade de regras que cada entidade pode criar (esse é o motor de regras já identificado como o ponto mais arriscado do projeto).
- **Cobrança pública + Portal coexistindo.** Hoje já existe link público para emissão de algumas cobranças, sem necessidade de login (principalmente a sindical, que é obrigatória mesmo para quem não está cadastrado). Isso deve continuar existindo no sistema novo, em paralelo ao Portal do Contribuinte, que cobre o caso de quem já tem cadastro e senha.

## Domínios identificados

Você pediu pra pensar nisso no estilo DDD — qual é o domínio **core** (onde está o diferencial e a complexidade real de negócio), o que é **suporte** (necessário e específico do negócio, mas não é o diferencial) e o que é **genérico** (capacidade comum, que até poderia ser resolvida com algo pronto). Segue uma proposta inicial, ainda aberta a ajuste:

### Core Domain

- **Contribuintes** — cadastro principal, endereços, contatos, vínculos, sócios, histórico mensal de capital social/funcionários. É o domínio que sustenta todo o resto.
- **Cobrança / Arrecadação** — motor de regras parametrizável, emissão de boletos, baixa automática/manual, multas e juros. É aqui que está a maior complexidade de negócio e o maior risco do projeto.

A Tela de Negociação e o Portal do Contribuinte não são domínios próprios — são as duas "fachadas" (uma interna, uma externa) que expõem o core para quem precisa consultar e agir sobre contribuinte + cobrança.

### Supporting Domain

- **Financeiro (interno)** — contas a pagar/receber do próprio sindicato com fornecedores. Específico do negócio, mas não é o diferencial: é o mesmo tipo de problema que qualquer empresa tem internamente, só que vinculado ao contexto de cada entidade.
- **Jurídico** — cadastro de processos, audiências e advogados. Específico do negócio (benefício de sócio, controle dos processos do sindicato), mas de baixa complexidade.
- **Mailing / Comunicação** — campanhas de e-mail e SMS. Precisa estar amarrado à base de contribuintes/sócios, mas não é onde está o diferencial do sistema.

### Generic Domain

- **Usuários e Permissões** — autenticação e controle de acesso. O mecanismo em si é genérico (existe em qualquer sistema); a única particularidade de negócio é o vínculo de um usuário a várias entidades com permissões diferentes em cada uma. Vale a discussão se essa particularidade já justifica tratar como suporte em vez de genérico — fica em aberto.
- **Relatórios** — capacidade comum a praticamente qualquer sistema, que depende dos outros domínios estarem corretos para gerar dados confiáveis. Como você mesmo notou, talvez seja o de menor prioridade justamente por isso.
- **Logs / Auditoria** — rastreabilidade de alterações. Genérico do ponto de vista de domínio, mas crítico como requisito não funcional — precisa estar presente desde o início, em todos os módulos.
- **Entidades (multi-tenant)** — "isolar dados por tenant" é uma capacidade técnica comum a qualquer sistema multi-tenant; a particularidade de negócio (quais CNAEs e cidades cada entidade abrange) é pequena. O cadastro/infraestrutura de entidade em si é mais infraestrutura genérica do que regra de negócio diferenciadora.
- **Cadastro de Documentos** — registro simples de documentos a assinar; não tem regra de negócio elaborada.

## Dependências entre domínios

```
Entidades (multi-tenant) ──┐
                            ├──> Contribuintes ──> Cobrança ──┬──> Tela de Negociação
Usuários/Permissões ───────┘                                 └──> Portal do Contribuinte

Financeiro / Jurídico / Mailing / Documentos → relativamente independentes entre si
```

## Por onde começar (proposta de fases)

1. **Fundação.** Fechar a decisão de front-end (Angular, Blazor ou MVC — o back-end em ASP.NET Core já está decidido), modelar Entidade e Usuário/Permissões, montar o esqueleto do projeto (camadas, padrões, pipeline) já com logs/auditoria previstos desde o início. Em paralelo, começar a desenhar a estratégia de migração — principalmente o mapeamento de como as entidades atuais serão "retrabalhadas" na nova estrutura, já que isso afeta todo o resto.
2. **Contribuintes.** Construir o domínio completo (cadastro, endereços, contatos, vínculos, sócios, histórico mensal). É o primeiro módulo a ser liberado para o cliente, segundo a estratégia de entrega incremental já combinada.
3. **Cobrança.** Atacar primeiro o desenho do motor de regras (a parte mais arriscada do projeto) antes de implementar a emissão/baixa de boletos.
4. **Tela de Negociação e Portal do Contribuinte.** Naturalmente vêm depois, já que dependem de Contribuinte e Cobrança prontos.
5. **Domínios de suporte.** Financeiro, Jurídico e Mailing podem ser paralelizados conforme a capacidade do time, sem bloquear o caminho crítico.

## Decisões em aberto que valem ser resolvidas logo

- Abordagem de front-end: back-end já definido como ASP.NET Core, mas a equipe está em dúvida entre Angular (SPA), Blazor, ou manter o padrão MVC tradicional que já é usado em outros projetos da empresa.
- Desenho do motor de regras de cobrança (como representar faixas, percentuais e critérios sem virar uma tela impossível de usar, mas tirando do Anderson a necessidade de codificar regra por regra).
- Estratégia de migração das entidades retrabalhadas (mapeamento do modelo antigo para o novo).
- Classificação final de Usuários e de Entidades como domínio genérico ou de suporte — discutido na seção de domínios, mas ainda em aberto.
- Como vai funcionar, na prática, o perfil de administrador da plataforma (só foi registrada a intenção de ter algo assim).

## Próximos passos

- Aguardando as próximas anotações para complementar este documento.
