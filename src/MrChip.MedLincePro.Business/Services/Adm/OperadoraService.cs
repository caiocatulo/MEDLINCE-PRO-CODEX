using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Adm;

public sealed class OperadoraService : ServiceBase, IOperadoraService
{
    private readonly IOperadoraRepository _repository;

    public OperadoraService(IOperadoraRepository repository, IUserContext userContext)
        : base(userContext)
    {
        _repository = repository;
    }

    public async Task<ResultadoOperacao<IEnumerable<OperadoraDto>>> ListarAsync(Guid? empresaId = null, bool liberadoParaAgendaMedica = false, bool somenteComWsHabilitado = false, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<IEnumerable<OperadoraDto>>.Falha(auth.Mensagem);
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<IEnumerable<OperadoraDto>>.Falha("Empresa inválida.");

        var dados = await _repository.ListarPorEmpresaAsync(empresa, liberadoParaAgendaMedica, somenteComWsHabilitado, ct);
        return ResultadoOperacao<IEnumerable<OperadoraDto>>.Ok(dados.Select(x => x.ToDto()));
    }

    public async Task<ResultadoOperacao<OperadoraDto?>> ObterPorIdAsync(Guid operadoraId, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<OperadoraDto?>.Falha(auth.Mensagem);
        if (operadoraId == Guid.Empty) return ResultadoOperacao<OperadoraDto?>.Falha("OperadoraId inválido.");
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<OperadoraDto?>.Falha("Empresa inválida.");

        var operadora = await _repository.ObterPorEmpresaAsync(empresa, operadoraId, ct);
        return ResultadoOperacao<OperadoraDto?>.Ok(operadora?.ToDto());
    }

    public async Task<ResultadoOperacao<OperadoraDto>> AdicionarAsync(OperadoraDto dto, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<OperadoraDto>.Falha(auth.Mensagem);
        Normalizar(dto);
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<OperadoraDto>.Falha("Empresa inválida.");
        if (string.IsNullOrWhiteSpace(dto.RazaoSocial)) return ResultadoOperacao<OperadoraDto>.Falha("Informe a razão social da operadora.");
        dto.OperadoraId = dto.OperadoraId == Guid.Empty ? Guid.NewGuid() : dto.OperadoraId;
        dto.Ativo = true;
        dto.DataCadastro = dto.DataCadastro == default ? DateTime.UtcNow : dto.DataCadastro;
        dto.EmpresaOperadora ??= new EmpresaOperadoraDto();
        dto.EmpresaOperadora.EmpresaId = empresa;
        dto.EmpresaOperadora.OperadoraId = dto.OperadoraId;
        dto.EmpresaOperadora.EmpresaOperadoraId = dto.EmpresaOperadora.EmpresaOperadoraId == Guid.Empty ? Guid.NewGuid() : dto.EmpresaOperadora.EmpresaOperadoraId;
        dto.EmpresaOperadora.Ativo = true;

        var ok = await _repository.AdicionarAsync(empresa, dto.ToEntity(), ct);
        return ok ? ResultadoOperacao<OperadoraDto>.Ok(dto, "Operadora criada/vinculada com sucesso.") : ResultadoOperacao<OperadoraDto>.Falha("Falha ao criar operadora.");
    }

    public async Task<ResultadoOperacao> AtualizarAsync(Guid operadoraId, OperadoraDto dto, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (operadoraId == Guid.Empty) return ResultadoOperacao.Falha("OperadoraId inválido.");
        Normalizar(dto);
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao.Falha("Empresa inválida.");
        dto.OperadoraId = operadoraId;
        dto.EmpresaOperadora ??= new EmpresaOperadoraDto();
        dto.EmpresaOperadora.EmpresaId = empresa;
        dto.EmpresaOperadora.OperadoraId = operadoraId;

        var ok = await _repository.AtualizarAsync(empresa, dto.ToEntity(), ct);
        return ok ? ResultadoOperacao.Ok("Operadora atualizada com sucesso.") : ResultadoOperacao.Falha("Operadora não encontrada para a empresa.");
    }

    public async Task<ResultadoOperacao> InativarAsync(Guid operadoraId, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (operadoraId == Guid.Empty) return ResultadoOperacao.Falha("OperadoraId inválido.");
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao.Falha("Empresa inválida.");

        var ok = await _repository.InativarVinculoAsync(empresa, operadoraId, ct);
        return ok ? ResultadoOperacao.Ok("Vínculo da operadora inativado com sucesso.") : ResultadoOperacao.Falha("Operadora não encontrada para a empresa.");
    }

    private static void Normalizar(OperadoraDto dto)
    {
        dto.RazaoSocial = Normalization.TrimOrEmpty(dto.RazaoSocial);
        dto.NomeFantasia = Normalization.TrimOrEmpty(dto.NomeFantasia);
        dto.Cnpj = Normalization.OnlyDigits(dto.Cnpj);
        dto.RegistroAns = Normalization.TrimOrEmpty(dto.RegistroAns);
        dto.ContatoEmail = Normalization.LowerTrimOrEmpty(dto.ContatoEmail);
    }
}
