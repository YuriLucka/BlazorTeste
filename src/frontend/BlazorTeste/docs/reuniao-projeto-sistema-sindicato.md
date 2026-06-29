# Reunião de Levantamento de Requisitos — Novo Sistema de Gestão para Sindicato

> **Nota sobre este documento:** este arquivo foi gerado a partir da transcrição automática de uma reunião gravada. O áudio original trazia trechos com ruído típico de transcrição (repetições, frases incompletas, palavras não reconhecidas corretamente, falhas de reconhecimento e conversas paralelas sem relação com o projeto). Para que o conteúdo fosse útil como registro do projeto, o texto foi reorganizado por tema, teve a pontuação e a gramática corrigidas, e trechos sem valor informativo (comentários pessoais, ruído de transcrição, partes ininteligíveis) foram removidos. Alguns termos técnicos foram interpretados pelo contexto quando a transcrição claramente os distorceu — por exemplo, "KINAI" foi interpretado como **CNAE** (classificação de atividade econômica), já que o próprio áudio o define como tal. Quando não havia certeza suficiente sobre um termo, ele foi descrito de forma mais genérica em vez de "adivinhado". Se for necessário o texto literal e completo da transcrição, posso gerar essa versão também.

## Resumo executivo

A DPi vai desenvolver um sistema novo de gestão para um cliente do segmento sindical, substituindo um sistema legado que está em uso há muitos anos. O sistema atual funciona, mas tem uma arquitetura tecnicamente datada (Web Forms, mistura de ORM com SQL direto) e cresceu de forma orgânica ao longo do tempo, acumulando complexidade. O novo sistema deve manter o conceito central de multi-tenant (uma única aplicação atendendo a mais de 50 sindicatos), reconstruir os módulos de contribuintes e cobrança com um motor de regras configurável, adicionar um portal externo para os contribuintes e modernizar módulos auxiliares (jurídico, financeiro interno, comunicação por e-mail/SMS), tudo isso com entrega incremental e migração completa dos dados existentes.

---

## 1. Contexto do projeto

A DPi vai desenvolver um novo sistema de gestão para um cliente do segmento sindical. O cliente já possui um sistema antigo, em uso há muitos anos, e quer substituí-lo por um sistema novo, com tecnologia atual e melhorias de arquitetura, processo e usabilidade. Nesta reunião, a pessoa responsável pelo sistema atual apresentou ao time da DPi como o sistema funciona hoje, quais são suas dores e o que se espera do sistema novo.

## 2. Diagnóstico do sistema atual

### 2.1 Arquitetura técnica

O sistema legado foi estruturado em torno do conceito de repositórios, com um ORM cuidando do acesso a dados. Na prática, porém, essa estrutura está incompleta: parte das consultas continua escrita diretamente em SQL (MySQL), com nomes de colunas e estrutura bastante antigos, misturando os dois padrões de acesso a dados.

O problema mais grave, no entanto, não é esse: a interface é feita em **Web Forms** (ASP.NET), um modelo de desenvolvimento web antigo, baseado em eventos de clique semelhante ao Windows Forms, com necessidade de IDs manuais para cada campo de tela. É uma tecnologia ultrapassada que limita a evolução do sistema.

### 2.2 Tamanho e complexidade

O sistema tem mais de 300 tabelas e cerca de 400 tabelas de relacionamento/ligação, além de mais de 50 sindicatos cadastrados (parte deles ativos hoje). Boa parte dessa complexidade vem de funcionalidades que foram sendo adicionadas ao longo dos anos sem um planejamento de arquitetura prévio — coisas que pareciam necessárias no momento e foram incorporadas, gerando uma estrutura que cresceu de forma orgânica. Isso não significa que o sistema tenha sido malfeito: ele cumpriu sua função, mas seu crescimento contínuo não permitiu reestruturações ao longo do tempo.

Outro ponto histórico relevante: por limitações de bancos de dados antigos (que permitiam nomes de tabela com poucos caracteres), diversas tabelas têm nomes pouco descritivos — por exemplo, a tabela de contribuintes é identificada internamente apenas pelo código "13". Isso deve ser levado em conta durante o mapeamento e a migração de dados para o sistema novo.

## 3. Diretrizes para o sistema novo

- **Critério de arquitetura.** O novo sistema vai rodar por muitos anos, então a escolha de tecnologia não deve ser feita apenas porque algo é "mais rápido", "mais bonito" ou "mais novo" — o critério principal é ter uma arquitetura consistente, sólida e alinhada a padrões que a equipe já domina, evitando problemas no longo prazo.
- **Tecnologia.** Ainda em discussão. As opções avaliadas foram manter um padrão mais próximo do que a equipe já utiliza, em ASP.NET Core, ou desenvolver o front-end em Angular consumindo uma API em .NET Core. Ao final da reunião, a equipe demonstrou inclinação por usar Angular, já que é uma tecnologia comum em ambientes corporativos e a equipe já tem alguma experiência prática com ela (com apoio de um colega que já trabalhou com Angular em outros projetos). A decisão final, porém, ainda exige mais discussão e planejamento antes do início do desenvolvimento.
- **Relatórios.** A preferência é por relatórios exibidos em tela, evitando a geração de PDF sempre que possível. Em especial, deve-se evitar o uso do Crystal Reports, que está desatualizado e não se integra bem a projetos novos em .NET Core.
- **Interface visual.** O sistema novo deve ter uma interface moderna e responsiva, incluindo uso confortável em celular — diferente do visual datado do sistema atual.
- **Entrega incremental.** O sistema não será entregue de uma só vez no final do projeto. Conforme cada módulo for ficando pronto (por exemplo, o cadastro de contribuintes), ele será liberado para o cliente, que vai recebendo e usando o sistema aos poucos, em vez de esperar a entrega final.
- **Migração de dados.** Será necessário migrar todos os dados existentes do sistema atual para o sistema novo.

## 4. Arquitetura multi-tenant (múltiplas entidades)

O novo sistema precisa ser multi-tenant: uma única instalação, rodando uma única vez, deve atender a múltiplas entidades (hoje há mais de 50 sindicatos cadastrados, com um subconjunto ativo no momento). Isso é diferente do modelo usado em outros produtos da empresa, em que cada cliente recebe uma instância própria — aqui, todos os sindicatos compartilham o mesmo sistema e a mesma base de dados.

No sistema atual esse conceito já existe: cerca de 90% das tabelas têm uma coluna de referência (`CD_SINDICATO`) que identifica a qual sindicato aquele registro pertence, e toda consulta feita em tela considera apenas os dados relacionados à entidade selecionada. Com o tempo, o sistema passou a atender não só sindicatos, mas também federações e confederações, então o conceito real é mais amplo do que "sindicato" — é uma **entidade**. A equipe já identificou que, no sistema novo, faz sentido renomear esse campo de referência para algo como `CD_ENTIDADE`, refletindo melhor o que ele representa.

Pontos importantes sobre esse modelo:

- **Usuários com acesso a múltiplas entidades.** Um mesmo usuário pode ter acesso a mais de um sindicato. É o caso, por exemplo, da equipe de São Paulo, que hoje trabalha tanto no sindicato de hotéis quanto no sindicato de bares e restaurantes — duas entidades que já foram um único sindicato e depois se dividiram, mas continuam com a mesma equipe atuando nas duas. Quando um usuário tem acesso a apenas uma entidade (como ocorre, por exemplo, com a equipe de Sorocaba), o sistema deve direcioná-lo automaticamente para ela, sem exigir seleção. Quando o usuário tem acesso a mais de uma, é necessária uma seleção — que pode estar já na tela de login, sem precisar de uma etapa separada.
- **Permissões por entidade, não globais.** As permissões de acesso a telas são configuradas por entidade, não de forma global para o usuário: um mesmo usuário pode ter acesso a uma tela em um sindicato e não ter acesso à mesma tela em outro. Hoje essas permissões funcionam por tela completa (e não por ação dentro da tela — ou o usuário tem acesso total à tela, ou não tem acesso nenhum), e não existe ainda um conceito de perfil de acesso (como "administrador"): qualquer usuário com acesso à tela de usuários pode cadastrar, alterar ou remover outros usuários livremente. A equipe identificou que isso provavelmente precisa ser revisto no sistema novo, com algum nível de restrição adicional além da simples habilitação de telas.
- **Base de abrangência de cada entidade.** Cada entidade tem uma área de atuação definida por códigos de CNAE e por cidades/regiões. Por exemplo, o sindicato de bares e restaurantes atua apenas sobre empresas com determinados códigos de CNAE (relacionados a alimentação), enquanto o sindicato de hotéis atua sobre outros códigos; da mesma forma, cada sindicato tem uma lista de cidades onde atua. Hoje esse cadastro existe apenas de forma manual, sem uma tela própria, e deveria ganhar um cadastro estruturado no sistema novo, possivelmente envolvendo duas entidades de cadastro relacionadas (a entidade sindical e sua base de abrangência).

## 5. Módulo de Contribuintes (núcleo do sistema)

O contribuinte é o coração do sistema: representa cada empresa filiada ou obrigada a contribuir com um determinado sindicato (no caso, sindicatos patronais, que representam empresas como restaurantes, bares, hotéis e promotoras de eventos na região de São Paulo). Em tese, toda empresa que se enquadra na categoria daquele sindicato é uma contribuinte obrigatória — por exemplo, ao abrir uma nova unidade de uma rede de fast food, o sindicato correspondente passa a cobrar contribuições dessa empresa desde a data de abertura, mesmo que ela nunca tenha se cadastrado formalmente.

### 5.1 Cadastro principal

O cadastro de contribuinte hoje (datado de 2010, mas ainda adequado para a época) reúne informações como:

- razão social e CNPJ da empresa;
- data de cadastro no sistema (automática) e data de abertura da empresa (usada para saber desde quando o sindicato deveria cobrar);
- capital social, usado como base de cálculo de diversas cobranças (uma empresa com capital social maior paga valores de boleto maiores);
- número de funcionários, também usado em diversos cálculos de cobrança;
- código de CNAE (atividade econômica), usado para confirmar que a empresa realmente pertence ao escopo daquele sindicato — o sistema atual filtra a lista de CNAEs disponíveis para cadastro de acordo com o sindicato em uso, evitando cadastros incorretos em uma lista com milhares de códigos possíveis;
- vínculos opcionais com outros agrupamentos (por exemplo, um contribuinte pode estar vinculado a um sindicato de segmento específico, com data de início e fim).

Um ponto importante levantado na reunião: tanto o número de funcionários quanto o capital social são valores que mudam com o tempo (uma empresa pode ter 10 funcionários em um mês e 12 no mês seguinte), mas o sistema atual guarda apenas o valor mais recente, sem histórico. Como as cobranças têm referência mensal e dependem desses valores, é importante que o sistema novo passe a manter um **histórico mensal** desses dados, em vez de apenas o valor atual.

### 5.2 Endereços

O contribuinte pode ter mais de um endereço — por exemplo, o do estabelecimento físico (fundamental para confirmar que a empresa realmente pertence à área de abrangência daquele sindicato) e endereços específicos para cobrança, correspondência ou questões jurídicas. No sistema atual essa separação não é clara: existe uma tabela de endereços vinculada ao contribuinte, mas sem deixar explícito qual é o endereço real do estabelecimento, o que já gerou casos de sindicatos cadastrando contribuintes que, na prática, não pertencem à sua área de atuação. No sistema novo, é necessário garantir que sempre haja um endereço de estabelecimento obrigatório, além de permitir múltiplos endereços adicionais com tipos definidos (cobrança, associativo, jurídico etc.).

### 5.3 Contatos e escritórios

Existe um cadastro de contatos vinculado ao contribuinte, hoje misturando telefone e e-mail em um único registro, quando o ideal seria separar contatos de e-mail e contatos telefônicos. Também é possível vincular um contribuinte a um escritório de contabilidade ou administração cadastrado no sistema — útil, por exemplo, para enviar boletos e correspondências de várias empresas para um único escritório responsável, em vez de enviar individualmente para cada contribuinte.

### 5.4 Sócios

Algumas empresas contribuintes também podem ser sócias do sindicato — uma condição distinta de ser contribuinte. Ser sócio dá acesso a benefícios do sindicato (como uso de áreas/recursos do sindicato e, no passado, assistência jurídica gratuita, benefício hoje descontinuado em alguns sindicatos) e implica uma cobrança associativa própria, com valor que pode variar de sócio para sócio. Cada sócio recebe uma matrícula própria, separada do código de contribuinte. A equipe está avaliando tratar essa condição como um tipo de **vínculo** do contribuinte (semelhante aos vínculos já citados), em vez de manter um cadastro totalmente separado como ocorre hoje.

## 6. Módulo de Cobrança e Arrecadação

Esse é, junto com o cadastro de contribuintes, o módulo mais crítico do sistema — praticamente tudo gira em torno de contribuintes e cobranças.

### 6.1 Tipos de cobrança

Cada sindicato pode ter diferentes tipos de cobrança, entre eles:

- **Sindical** — contribuição obrigatória de natureza legal/governamental, calculada anualmente com base no capital social da empresa, dentro de faixas que mudam ano a ano (a equipe mantém uma tabela de faixas para cada ano desde 2005). É a única cobrança que usa um boleto com layout específico, emitido pela Caixa Econômica, que distribui automaticamente os valores recebidos entre o sindicato, a federação, a confederação e o governo — por isso ela não usa o mesmo padrão de boleto bancário das demais cobranças e tem regras próprias de geração e baixa. Como exemplo de como a faixa funciona (dados apresentados na demonstração, referentes à tabela de 2026): capital social até determinado valor paga uma contribuição mínima fixa; faixas intermediárias aplicam um percentual sobre o capital social, às vezes somado a uma parcela adicional fixa; e há uma faixa de teto, em que empresas com capital social acima de um valor muito alto pagam um valor fixo, independentemente de quanto excedam esse teto.
- **Confederativa** — calculada com base no número de funcionários, por faixas ou, em alguns sindicatos, por valor multiplicado pelo número de funcionários.
- **Associativa** — cobrança mensal vinculada à condição de sócio.
- **Negocial** e **negocial mensal** — cobranças com regras mais variadas, que podem depender do regime tributário da empresa (simples nacional, lucro presumido etc.), do número de funcionários ou de outros critérios definidos pelo sindicato. Existem variações específicas, como contribuintes que negociam pagar o ano inteiro de uma vez (de janeiro a dezembro) em vez de mês a mês, ou que negociam valores diferentes do padrão.

Cada sindicato pode ter suas próprias regras e nomes para essas cobranças, e os valores também mudam de ano a ano.

### 6.2 Necessidade de um motor de regras parametrizável

Hoje, as regras de cálculo de cada cobrança (faixas de capital social, faixas de número de funcionários, percentuais, valores fixos por regime tributário etc.) estão codificadas diretamente no sistema. Toda vez que um sindicato muda uma faixa de valor ou cria uma cobrança nova, é necessário alterar o código, recompilar e publicar o sistema — um processo lento e arriscado, que já gerou pedidos de alteração em datas críticas, como véspera de fim de ano.

A ideia para o sistema novo é criar um **cadastro de cobranças** que permita configurar essas regras sem alteração de código: o nome da cobrança, quem deve pagá-la (todos os contribuintes, apenas contribuintes vinculados a determinado grupo, apenas contribuintes de determinado CNAE etc.) e como calcular o valor (por faixa de capital social, por faixa de número de funcionários, por percentual sobre algum desses valores, por regime tributário, entre outras combinações). Esse motor de regras foi identificado como a parte mais desafiadora do projeto, já que as regras variam bastante entre sindicatos e ao longo do tempo. Há também uma preocupação em equilibrar flexibilidade com simplicidade: parametrizar demais pode tornar a tela de cadastro de cobranças complexa demais para o time interno usar no dia a dia.

### 6.3 Multas e juros

Cada cobrança pode ter sua própria regra de multa e juros sobre o atraso:

- a **multa** é cobrada uma única vez sobre o valor da cobrança em atraso, independentemente de quanto tempo o contribuinte demore para pagar;
- o **juro** é calculado por período de atraso (em geral, por mês), incidindo continuamente até o pagamento.

Esses valores precisam ser recalculados em tempo real quando o contribuinte ou o atendente consulta o que está sendo devido, exibindo o valor original, a multa e o juro acumulado até a data da consulta.

### 6.4 Emissão de boletos

A geração de boletos das cobranças "normais" (associativa, confederativa, negocial etc.) segue o padrão comum de boleto bancário, hoje gerado com uma biblioteca .NET própria — diferente da cobrança sindical, que exige o boleto específico da Caixa e tem regra própria: cada ano deve gerar um boleto separado, sem possibilidade de somar valores de anos diferentes em um único boleto.

A meta para o sistema novo é automatizar ao máximo a geração de boletos — por exemplo, gerar e enviar automaticamente, todo mês ou conforme a regra de cada cobrança, sem depender de uma ação manual da equipe do sindicato para cada lote. Ainda assim, deve ser possível gerar boletos manualmente para um contribuinte específico, por exemplo quando o responsável por uma empresa liga pedindo a segunda via.

Quando o contribuinte tem mais de uma cobrança em aberto e quer dividir o valor total em parcelas, o sistema deve agrupar os valores de forma equilibrada entre as parcelas (e não simplesmente dividir o total por igual), priorizando sempre as cobranças mais antigas. Isso evita um problema do sistema atual: se o contribuinte paga apenas uma das parcelas de um pagamento dividido, não há como saber com certeza a quais cobranças específicas aquele valor corresponde, dificultando a baixa correta.

Por ora, o sistema deve continuar oferecendo apenas boleto bancário tradicional. A emissão de boleto com PIX (ou algo equivalente a um "débito automático" via PIX) foi mencionada como possível evolução futura, mas ainda não está definida e depende de conversa adicional com o cliente.

### 6.5 Baixa (registro de pagamento)

A baixa de boletos pode ser manual (quando o pagamento foi feito por outro meio, como em dinheiro, e precisa ser registrado manualmente) ou automática (a partir do arquivo de retorno enviado pelo banco). Hoje existem telas separadas de baixa automática e manual para cada tipo de cobrança (associativa, confederativa, assistencial, negocial etc.), o que gera duplicidade desnecessária — a meta é unificar esse processo em um fluxo único de baixa por banco, já que o procedimento técnico (ler o arquivo de retorno e confirmar os pagamentos) é o mesmo, independentemente do tipo de cobrança.

Cada sindicato pode trabalhar com mais de um banco (por exemplo, Santander e Itaú simultaneamente), e o banco usado normalmente está associado ao tipo de cobrança (por exemplo, a cobrança assistencial sempre sai pelo Santander). A equipe está avaliando se vale a pena tornar essa associação mais flexível.

Cada cobrança, cada boleto e cada baixa precisam manter um histórico completo e rastreável: quais cobranças geraram quais boletos, se cada boleto foi enviado, qual remessa o contemplou, se houve retorno do banco e como (e por quem) cada pagamento foi baixado. O sistema atual já faz isso, mas é importante manter esse nível de rastreabilidade no sistema novo.

## 7. Tela de Negociação

A tela de negociação é, hoje, a tela mais usada pela equipe do sindicato. Nela, ao informar o CNPJ de uma empresa, é possível ver tudo o que ela está devendo (de qualquer tipo de cobrança), com os valores de multa e juros já recalculados até a data atual, e gerar os boletos necessários, inclusive permitindo dividir o total em parcelas equilibradas como descrito acima.

Antes de negociar, o sistema pede que os dados cadastrais do contribuinte sejam atualizados (capital social, número de funcionários, regime tributário, endereço do estabelecimento, e-mail e telefone), já que esses dados influenciam diretamente o cálculo dos valores devidos.

Pontos a melhorar identificados pela equipe para o sistema novo:

- a tela deveria reunir, em um único lugar, tanto o que está pendente quanto o histórico completo do que já foi pago (hoje é necessário consultar outra tela para isso) — a ideia é que, quando alguém ligar para o sindicato, baste abrir essa única tela para atualizar dados e ver o histórico completo de cobranças, pagamentos e negociações;
- não existe hoje um histórico das negociações realizadas: se um contribuinte negocia um acordo de pagamento e depois não cumpre, não há como consultar os termos daquela negociação específica — é necessário recomeçar do zero. O sistema novo deve manter esse histórico;
- a tela também precisa emitir certidões de débito: CND (certidão negativa de débito, quando o contribuinte não tem nenhuma pendência) e CPD (certidão positiva, listando o que está pendente). Empresas costumam precisar dessas certidões, por exemplo, ao serem vendidas ou encerradas.

## 8. Comunicação por E-mail e SMS (Mailing)

O sistema atual tem um módulo próprio de campanhas de e-mail e SMS, usado para enviar comunicados em massa aos contribuintes ou aos sócios. A vantagem de manter esse recurso dentro do próprio sistema (em vez de usar um serviço externo) é que a base de contribuintes e seus dados de contato já está nele.

O funcionamento atual é, basicamente: o usuário cria uma campanha (com base em um e-mail anterior ou do zero), escreve ou importa o conteúdo em HTML, anexa arquivos se necessário (como editais), seleciona os destinatários (todos os contribuintes com e-mail cadastrado — em um dos sindicatos, por exemplo, há mais de 66 mil contribuintes cadastrados, com uma parte deles tendo e-mail —, contatos específicos, ou contatos de um cadastro auxiliar de pessoas/empresas que não são contribuintes) e dispara o envio. O sistema espera alguns segundos entre cada envio para evitar bloqueios por spam, e tenta rastrear se cada e-mail foi entregue e aberto, usando uma imagem invisível com identificador único embutida no e-mail — técnica que hoje tem eficácia limitada, já que a maioria dos clientes de e-mail bloqueia o carregamento automático de imagens.

A principal limitação do sistema atual é que o disparo de e-mails é **síncrono**: o processo roda na própria tela enquanto o usuário espera, sem processamento em segundo plano. Se a tela travar ou for fechada antes de concluir o envio de todos os e-mails, não há como retomar o envio do ponto em que parou — é necessário recomeçar, mesmo sem confirmação de quem realmente recebeu.

No sistema novo, a recomendação é reconstruir esse módulo com processamento em segundo plano (fila/background job), garantindo que o envio continue de forma confiável mesmo em bases grandes, e mantendo relatórios de status (enviados, taxa de entrega, taxa de abertura, taxa de cliques). Por ora, o cliente não solicitou integração com WhatsApp, mas isso poderia ser avaliado como evolução futura.

## 9. Módulo Financeiro (interno do sindicato)

Esse módulo é separado da arrecadação dos contribuintes: trata das finanças internas do próprio sindicato, como pagamentos a fornecedores e outras despesas operacionais. O que existe hoje é bastante limitado (praticamente só um controle simples do que falta pagar), então a equipe terá que construir esse módulo praticamente do zero — mas com escopo simples, sem necessidade de um sistema financeiro completo, apenas o controle básico do que precisa ser pago e do que precisa ser recebido.

Funcionalidades esperadas:

- cadastro de categorias de despesa, contas bancárias do sindicato e fornecedores ("favorecidos");
- cadastros opcionais de centro de custo e histórico/padrões de rateio entre projetos;
- lançamento de movimentações financeiras (pagamentos e recebimentos), incluindo lançamentos recorrentes;
- relatório de fluxo de caixa, mostrando o que foi realizado e o que está previsto, mês a mês, por conta;
- conciliação bancária — funcionalidade considerada interessante de incluir, ainda que dependente das particularidades de cada banco.

Um ponto a observar: o que entra como "a receber" no fluxo de caixa não pode ser simplesmente o total bruto emitido em boletos das cobranças, já que parte significativa não é paga. O fluxo de caixa precisa refletir, de alguma forma, a expectativa real de recebimento dos boletos emitidos aos contribuintes, e não apenas o valor total emitido.

## 10. Módulo Jurídico

O módulo jurídico, hoje, é um cadastro relativamente simples, desenvolvido rapidamente, mas que continua sendo usado e atende bem ao que precisa. Ele inclui:

- cadastro de advogados (com e-mail e demais dados de contato);
- cadastro de processos, com número do processo, tipo de processo/ordem, tipo de tribunal, vara, situação, dados de procuração e origem;
- cadastro de audiências e trâmites relacionados a cada processo;
- log de alterações, permitindo saber quem cadastrou ou alterou cada processo;
- relatório de pauta do advogado, filtrando as audiências previstas por dia.

Um benefício relevante para as empresas que são sócias do sindicato é o acesso à assistência jurídica gratuita por meio desse módulo (hoje descontinuada em alguns sindicatos, mas historicamente um dos diferenciais de ser sócio).

Esse módulo já recebeu pedidos pontuais de atualização por parte do cliente e deverá ser revisado durante o projeto, mas seu escopo é simples e não deve fugir muito do padrão básico de cadastro e consulta.

## 11. Cadastro de Documentos (funcionalidade não solicitada, mas necessária)

A equipe identificou uma necessidade que o cliente não chegou a pedir explicitamente, mas que deve fazer parte do sistema novo: um cadastro de documentos que as empresas contribuintes precisam assinar — hoje, esse processo é feito de forma manual, com documentos gerados em Word e formalizados separadamente. A ideia é ter um cadastro simples vinculando cada empresa ao(s) documento(s) que ela precisa assinar, com o registro da assinatura, sem necessidade de reinventar todo o processo — apenas estruturar de forma mais organizada o que hoje é feito de maneira dispersa.

## 12. Portal do Contribuinte (acesso externo)

Hoje, o sistema é usado apenas pela equipe interna dos sindicatos — não há nenhuma forma das empresas contribuintes acessarem diretamente seus próprios dados, exceto por páginas públicas isoladas e específicas para cada tipo de cobrança (por exemplo, uma página apenas para emissão da sindical, outra apenas para a negocial, cada uma em um endereço diferente e sem login). Essas páginas públicas permitem que a empresa informe o CNPJ e tire o boleto correspondente, mas sem mostrar se ela já pagou ou não — o que já gerou casos de empresas tirando e pagando o mesmo boleto duas vezes.

O cliente já recebeu a promessa de que o sistema novo terá um **portal do contribuinte** de fato, com login (provavelmente por CNPJ e senha), no qual a empresa poderá:

- conferir e atualizar seus próprios dados cadastrais;
- ver, em uma tela única, todas as cobranças pendentes e já pagas, com valores atualizados (semelhante à tela de negociação usada internamente, mas sem permitir que o contribuinte altere valores);
- emitir e pagar os boletos pendentes.

Caso exista uma negociação especial registrada (feita em uma "mesa de negociação" com a equipe do sindicato), essa informação também deve aparecer para o contribuinte no portal.

Uma exceção permanece: a emissão da cobrança sindical precisa continuar disponível também por um link público, sem exigir cadastro prévio ou senha, já que mesmo empresas que nunca se cadastraram formalmente no sindicato são obrigadas a pagar essa contribuição.

## 13. Escopo, integrações e escala

- **Quem usa o sistema hoje.** Apenas a equipe interna dos sindicatos (colaboradores); não há acesso externo para as empresas contribuintes — esse é justamente o problema que o portal do contribuinte (seção 12) deve resolver.
- **Integração bancária.** O sistema não tem uma integração via API completa com os bancos. A geração de boleto, o envio de remessa e a leitura do arquivo de retorno são feitos por meio de uma biblioteca (DLL) .NET própria, que já conhece os dados e o layout de cada banco. Essa abordagem deve ser avaliada/mantida no sistema novo.
- **Fora do escopo.** Existe, no sistema atual, um módulo de gestão de eventos usado por outro sindicato, mais complexo e fora do escopo deste projeto — não será migrado nem desenvolvido nesta etapa.
- **Escala aproximada.** Mais de 50 sindicatos cadastrados, mais de 300 tabelas e cerca de 400 tabelas de relacionamento no banco de dados, e dezenas de milhares de contribuintes (apenas em um dos sindicatos, mais de 66 mil contribuintes cadastrados).

## 14. Próximos passos

- A equipe da DPi vai receber acesso ao projeto/repositório do sistema atual para explorar a estrutura existente em mais detalhes.
- A decisão final de arquitetura e tecnologia (ASP.NET Core vs. Angular + API .NET Core) ainda precisa ser amadurecida com calma antes do início do desenvolvimento.
- O motor de regras de cobrança (seção 6.2), por ser a parte mais desafiadora do projeto, deve receber atenção e planejamento extra antes da implementação.
- A entrega será incremental, começando pelo cadastro de contribuintes, seguido pelos módulos de cobrança e demais módulos, com liberação progressiva ao cliente conforme cada parte for concluída.
- A migração de dados de todas as tabelas existentes precisa ser planejada em paralelo ao desenvolvimento dos novos módulos.
