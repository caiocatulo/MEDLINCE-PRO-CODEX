# Análise do legado e plano de migração

## Escopo analisado

Solution legada anexada: `api.mrchip.medlince.sln`.

Projetos identificados:

1. `MrChip.Api.MedLince.Business`
2. `MrChip.Api.MedLince.Data`
3. `MrChip.MedLince.Api`

Métricas da inspeção do pacote:

- 349 arquivos no pacote.
- 331 arquivos C#.
- Aproximadamente 30 mil linhas C#.
- 26 controllers identificados:
  - `V1-ADM`: 14
  - `V1-APP`: 6
  - `V1-ANS`: 1
  - `V1-Auth`: 1
  - `V1-DM`: 1
  - `V1-PP`: 1
  - `V1-TISS`: 1
  - `VI-BI`: 1
- Target framework atual dos projetos legados: `net8.0`.
- Uso encontrado de Dapper no legado:
  - `QueryAsync`: 67 ocorrências.
  - `QueryFirstOrDefaultAsync`: 21 ocorrências.
  - `ExecuteAsync`: 50 ocorrências.
  - `Query<T>` síncrono: 143 ocorrências.

## Achados técnicos principais

### 1. Controllers com dependência direta de repositories

Há controllers acionando repositories diretamente. Para a nova API, a regra definida é:

- Controller recebe requisição.
- Controller valida contrato de entrada básico.
- Controller chama Service.
- Service executa regra de negócio.
- Repository executa acesso a dados.

**Risco:** Médio/Alto.

Motivo: regras espalhadas dificultam teste, manutenção e controle de transação.

### 2. Mistura de Dapper async, Dapper síncrono e ADO.NET legado

Foram encontrados padrões com `SqlConnection`, Dapper e trechos legados síncronos.

Na nova base:

- Usar `QueryAsync`.
- Usar `QueryFirstOrDefaultAsync`.
- Usar `ExecuteAsync`.
- Evitar `DataTable` e `SqlDataAdapter`.

**Risco:** Médio.

Motivo: chamadas síncronas em API podem aumentar bloqueio de threads sob carga.

### 3. Segredos e compatibilidade de senha no código legado

O legado contém configurações sensíveis/hardcoded. Esses valores não foram copiados para a nova solution.

Na nova base:

- Senha SQL vem de variável de ambiente.
- Chave de compatibilidade da senha legada vem de configuração externa.
- JWT usa chave de ambiente/Key Vault em produção.

**Risco:** Alto.

Motivo: segredo em repositório é risco direto de produção e compliance.

### 4. Projeções SQL legadas

As projeções principais dos repositories de ADM foram preservadas como base inicial, sem adicionar/remover colunas de forma arbitrária.

**Regra mantida:** não alterar SELECT legado apenas porque uma classe C# possui mais ou menos propriedades.

**Risco:** Médio.

Motivo: ainda é obrigatório validar contra schema real em DEV/HML antes do PR.

### 5. Versionamento e rotas

O legado possui rotas por versão e algumas inconsistências de nomenclatura. A base nova inicia com rotas explícitas `api/v1` sem introduzir pacote de versionamento antes da necessidade real.

**Risco:** Baixo.

Motivo: reduz complexidade inicial. Pode ser evoluído depois com versionamento formal.

## Decisões adotadas na nova base

### Camada Business

Contém:

- Entidades ADM.
- DTOs.
- Interfaces de Services e Repositories.
- Services com validações simples.
- Serviço de compatibilidade de senha legada sem segredo hardcoded.

### Camada Data

Contém:

- `SqlConnectionFactory` centralizada.
- Repositories ADM.
- Dapper async.
- Transações onde há gravação em mais de uma tabela.

### Camada Api

Contém:

- Controllers finos.
- JWT Bearer.
- Swagger.
- Rate limit de login.
- Middleware global de exceções.
- `IUserContext` baseado nas claims do token.
- Autorização customizada por claims preparada para evolução.

## CRUDs incluídos

Banco: `DB_MEDLINCE_ADM`.

- Empresa
- Bureau
- Usuario
- Profissional
- UnidadeHospitalar
- Operadora

## Itens propositalmente não migrados nesta etapa

- Funcionalidades administrativas fora do escopo inicial.
- `DB_MEDLINCE_APP`.
- `DB_MEDLINCE_ANS`/`DB_MEDLINCE_AND`.
- Reescrita completa de todos os controllers legados.
- Alterações profundas de arquitetura.
- Troca para SPA/Blazor/React/Vue.

## Plano incremental recomendado

### PR 1 — Base API ADM

Branch:

```bash
feature/medlincepro-api-base
```

Conteúdo:

- Solution nova.
- Autenticação JWT.
- CRUDs ADM básicos.
- Configuração segura.
- Swagger.

### PR 2 — Endurecimento de segurança

- Revisar claims reais por perfil.
- Aplicar policies nos endpoints administrativos.
- Remover exposição de campos sensíveis em respostas, especialmente senhas/tokens de operadora.
- Definir rotação de segredo JWT.

### PR 3 — Validação contra banco DEV/HML

- Validar projeções SQL.
- Validar nomes de colunas e tipos.
- Validar constraints.
- Validar índices para filtros mais usados.

### PR 4 — DB_MEDLINCE_APP

- Migrar apenas consultas/CRUDs necessários para a PWA médico e módulos planejados.
- Separar regras administrativas das regras do app médico.

### PR 5 — DB_MEDLINCE_ANS/AND

- Somente leitura.
- Repositories read-only.
- Nenhum endpoint de escrita.

## Checklist antes de produção

- [ ] Validar build com SDK .NET 10 instalado.
- [ ] Rodar testes de autenticação com usuário real de DEV.
- [ ] Confirmar compatibilidade de senha legada com chave externa.
- [ ] Revisar campos sensíveis em DTOs de resposta.
- [ ] Validar todas as projeções SQL com schema real.
- [ ] Configurar Key Vault/App Configuration no Azure.
- [ ] Garantir que `appsettings*.json` não possui segredo real.
- [ ] Abrir Pull Request para `dev`.
- [ ] Executar smoke test em HML antes de produção.

## Classificação geral de risco

| Item | Risco | Observação |
|---|---:|---|
| Segredos no legado | Alto | Não foram copiados. Exige rotação/validação. |
| Compatibilidade de senha legada | Alto | Depende da chave correta fora do código. |
| Projeções SQL | Médio | Mantidas como base, mas precisam validação contra banco. |
| Controllers legados com repositories | Médio | Corrigido na base nova para o escopo ADM. |
| Rotas `api/v1` simples | Baixo | Reduz complexidade inicial. |
