# MrChip.MedLincePro.Api

Base inicial para a nova API MedLince Pro, criada a partir da análise da solution legada `api.mrchip.medlince.sln`.

## Estrutura

```text
MrChip.MedLincePro.Api.sln
├── src/
│   ├── MrChip.MedLincePro.Business
│   ├── MrChip.MedLincePro.Data
│   └── MrChip.MedLincePro.Api
└── docs/
    └── ANALISE_LEGADO_E_PLANO_MIGRACAO.md
```

## Ordem de construção adotada

1. **Business**
   - Entidades de domínio ADM.
   - DTOs.
   - Interfaces de Services e Repositories.
   - Services com validação de regra de negócio.
   - Compatibilidade controlada com senha legada.

2. **Data**
   - Repositories com Dapper async.
   - `SqlConnectionFactory` centralizada.
   - Sem `SqlDataAdapter`, sem `DataTable`.
   - Projeções SQL principais mantidas com base no legado.

3. **Api**
   - Controllers enxutos.
   - JWT Bearer.
   - Swagger com suporte a Bearer token.
   - Rate limit para autenticação.
   - Middleware global de exceções.
   - ViewModels somente onde existe forma de API diferente da regra de negócio.

## CRUDs iniciais

Banco inicial: `DB_MEDLINCE_ADM`.

- Empresa
- Bureau
- Usuario
- Profissional
- UnidadeHospitalar
- Operadora

## Configuração obrigatória

Nunca commitar segredos reais em `appsettings*.json`.

Variáveis recomendadas:

```bash
MEDLINCE_SQL_PASSWORD=senha_sql
PasswordCompatibility__LegacyTripleDesKey=chave_legada_validada_com_time
Jwt__Key=secret_forte_com_32_ou_mais_caracteres
```

## Execução local

Requer SDK .NET 10 instalado.

```bash
dotnet restore MrChip.MedLincePro.Api.sln
dotnet build MrChip.MedLincePro.Api.sln
dotnet run --project src/MrChip.MedLincePro.Api/MrChip.MedLincePro.Api.csproj
```

## Endpoint de autenticação

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "login": "usuario",
  "senha": "senha"
}
```

## Branch sugerida

```bash
git checkout dev
git pull
git checkout -b feature/medlincepro-api-base
```

Abrir Pull Request para `dev` após validação local e validação contra banco DEV/HML.
