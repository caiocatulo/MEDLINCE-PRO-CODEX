# Conferência DB_MEDLINCE_ADM x MrChip.MedLincePro.Api

## Escopo analisado

Script recebido: `banco-DB_MEDLINCE_ADM.sql`.

Banco declarado no script:

```sql
USE [DB_MEDLINCE_ADM]
```

Total de tabelas identificadas no script: **43**.

Tabelas ajustadas no primeiro ciclo ADM:

- `dbo.Empresa`
- `dbo.Bureau`
- `dbo.Usuario`
- `dbo.Profissional`
- `dbo.UnidadeHospitalar`
- `dbo.Operadora`
- tabelas auxiliares diretamente usadas pelos CRUDs:
  - `dbo.Endereco`
  - `dbo.EmpresaUnidadeHospitalar`
  - `dbo.EmpresaOperadora`
  - `dbo.BureauProfissional`
  - `dbo.UsuarioClaims`

## Correções aplicadas

### 1. Endereco

A classe `Endereco` herdava de `Entity`, mas a tabela `dbo.Endereco` não possui coluna `Ativo`.

Correção:

- `Endereco` deixou de herdar `Entity`.
- `EnderecoDto` deixou de expor `Ativo`.
- mantida `DataCadastro`, pois a coluna existe no banco.

Risco anterior: **Médio**. O DTO retornava uma informação que não existia no banco.

### 2. Empresa

A tabela `dbo.Empresa` possui colunas que não estavam representadas no modelo/DTO:

- `NumeroGuiaPrestador`
- `PercentualPIS`
- `PercentualCOFINS`
- `PercentualCSLL`
- `PercentualIR`
- `PercentualINSS`
- `PercentualISS`
- `TaxaAdm`

Correção:

- adicionadas propriedades no `Empresa` e `EmpresaDto`.
- ajustado `EmpresaRepository` em `SELECT`, `INSERT` e `UPDATE`.
- ajustada nulabilidade conforme schema.

Risco anterior: **Médio**. Campos financeiros/fiscais poderiam ser ignorados ou perdidos em atualização.

### 3. Usuario

A coluna `CodigoRecuperaSenha` é nullable no banco:

```sql
[CodigoRecuperaSenha] [int] NULL
```

Correção:

- `Usuario.CodigoRecuperaSenha` alterado para `int?`.
- `UsuarioDto.CodigoRecuperaSenha` alterado para `int?`.
- `UsuarioUpdateDto.CodigoRecuperaSenha` alterado para `int?`.

Risco anterior: **Baixo/Médio**. Valor nulo poderia ser materializado como zero, mudando a semântica.

### 4. Profissional

A tabela `dbo.Profissional` possui vários campos opcionais:

- `Cpf`
- `Email`
- `Telefone`
- `EnderecoComplemento`
- `CodigoGrauDeParticipacao`
- `BancoCodigo`
- `BancoAgencia`
- `BancoConta`
- `TipoDeDocumento`

Correção:

- ajustada nulabilidade na entidade e DTO.
- normalização agora converte opcionais vazios para `null`.
- vínculo `BureauProfissional.Matricula` passa a usar fallback para `Registro` quando CPF estiver ausente.

Risco anterior: **Médio**. CPF vazio poderia gerar falsa duplicidade e matrícula nula poderia quebrar insert no vínculo.

### 5. UnidadeHospitalar e EmpresaUnidadeHospitalar

A tabela `dbo.EmpresaUnidadeHospitalar` exige colunas obrigatórias:

- `Id`
- `EmpresaId`
- `CodigoCnes`
- `Nome`
- `Ativo`
- `DataCadastro`

O repository anterior inseria somente:

- `EmpresaId`
- `CodigoCnes`
- `Ativo`
- `DataCadastro`

Correção:

- `UnidadeHospitalarRepository.AdicionarAsync` agora informa `Id` e `Nome`.
- `AtualizarAsync` também atualiza o nome no vínculo.
- `Token` é preservado no update quando não informado.
- `Token` não é retornado no DTO de resposta.

Risco anterior: **Alto**. O insert do vínculo falharia por colunas NOT NULL sem valor.

### 6. Operadora e EmpresaOperadora

Divergências encontradas:

- `Operadora.CarteiraMin` e `Operadora.CarteiraMax` são `NOT NULL`, mas estavam como `int?`.
- `EmpresaOperadora` no código usava `Nomefantasia`; schema usa `NomeFantasia`.
- O código usava colunas inexistentes no schema:
  - `DiaMesFechamento`
  - `MaxMesesReenvio`
- A tabela possui colunas ausentes no modelo/repository:
  - `TagNumeroGuiaPrestador`
  - `HorarioObrigatorioXml`
  - `FormaCalculoProcedimento`
  - `ModeloGuiaAuxiliar`
  - `ConcatenarGrupoProcedimento`
- A coluna no banco é `carteiraObrigatoria` com inicial minúscula.

Correção:

- `CarteiraMin` e `CarteiraMax` alterados para `int`.
- removidas referências a colunas inexistentes.
- adicionadas colunas ausentes no modelo, DTO, mapping, `SELECT`, `INSERT` e `UPDATE`.
- `carteiraObrigatoria` tratada com alias para `CarteiraObrigatoria`.
- `WsSenha` é preservado no update quando não informado.
- `WsSenha` não é retornado no DTO de resposta.
- `Token` de operadora também não é retornado no DTO de resposta.

Risco anterior: **Alto**. Consultas/updates em colunas inexistentes quebrariam em runtime.

### 7. Swagger/OpenAPI e dependências

Correções complementares mantidas neste pacote:

- `Program.cs` atualizado para `Microsoft.OpenApi` atual.
- `OpenApiSecurityRequirement` ajustado para Swashbuckle/OpenAPI atual.
- `System.IdentityModel.Tokens.Jwt` atualizado para `8.16.0` para evitar downgrade com `JwtBearer 10.0.9`.
- removidas referências desnecessárias do projeto API que geravam `NU1510`.

Risco anterior: **Baixo/Médio**. Build poderia falhar no Visual Studio por conflito de pacote.

### 8. Configuração local

`appsettings.Development.json` foi mantido sem segredo real, usando placeholders.

Valores reais devem continuar em:

- `dotnet user-secrets`
- variável de ambiente
- Key Vault/cofre de segredo em Azure

Risco anterior se commitar segredo: **Alto**.

## Arquivos alterados

### Business

- `Models/Adm/Endereco.cs`
- `Models/Adm/Empresa.cs`
- `Models/Adm/Bureau.cs`
- `Models/Adm/Usuario.cs`
- `Models/Adm/Profissional.cs`
- `Models/Adm/UnidadeHospitalar.cs`
- `Models/Adm/Operadora.cs`
- `Dtos/Adm/EnderecoDto.cs`
- `Dtos/Adm/EmpresaDto.cs`
- `Dtos/Adm/BureauDto.cs`
- `Dtos/Adm/UsuarioDto.cs`
- `Dtos/Adm/ProfissionalDto.cs`
- `Dtos/Adm/UnidadeHospitalarDto.cs`
- `Dtos/Adm/OperadoraDto.cs`
- `Mapping/AdmMappingExtensions.cs`
- `Mapping/DtoToEntityExtensions.cs`
- `Services/Normalization.cs`
- `Services/Adm/ServiceBase.cs`
- `Services/Adm/EmpresaService.cs`
- `Services/Adm/BureauService.cs`
- `Services/Adm/ProfissionalService.cs`
- `Services/Adm/UnidadeHospitalarService.cs`
- `Services/Adm/OperadoraService.cs`

### Data

- `Repositories/Adm/EmpresaRepository.cs`
- `Repositories/Adm/BureauRepository.cs`
- `Repositories/Adm/UsuarioRepository.cs`
- `Repositories/Adm/ProfissionalRepository.cs`
- `Repositories/Adm/UnidadeHospitalarRepository.cs`
- `Repositories/Adm/OperadoraRepository.cs`

### Api

- `Program.cs`
- `MrChip.MedLincePro.Api.csproj`
- `appsettings.Development.json`

## Validação estática realizada

Foram conferidos:

- colunas usadas com aliases em repositories contra schema do SQL recebido;
- colunas de `INSERT` contra schema;
- colunas `NOT NULL` sem default contra listas de `INSERT`;
- remoção de referências a colunas inexistentes em `EmpresaOperadora`;
- remoção de `Endereco.Ativo`.

## Validação pendente

Não foi possível executar `dotnet restore/build` neste ambiente porque o SDK `dotnet` não está disponível no container.

Validação obrigatória no Visual Studio/PowerShell:

```powershell
cd C:\Projetos\0A-Producao\MEDLINCE-API-CODEX\MrChip.MedLincePro.Api

dotnet restore .\MrChip.MedLincePro.Api.sln
dotnet build .\MrChip.MedLincePro.Api.sln
```

Depois testar em ordem:

1. autenticação;
2. `GET /api/v1/adm/empresas`;
3. CRUD Empresa;
4. CRUD Bureau;
5. CRUD Usuario;
6. CRUD Profissional;
7. CRUD UnidadeHospitalar;
8. CRUD Operadora.

## Risco geral da alteração

**Risco: Médio.**

Motivo: a alteração é necessária para compatibilizar o código com o schema real, mas afeta DTOs, entidades e repositories. A maior redução de risco está nas correções de `EmpresaOperadora` e `EmpresaUnidadeHospitalar`, que evitam falhas diretas de SQL em runtime.
