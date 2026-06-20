using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Adm;

public sealed class EmpresaService : ServiceBase, IEmpresaService
{
    private readonly IEmpresaRepository _repository;
    private readonly ILogger<EmpresaService> _logger;

    public EmpresaService(IEmpresaRepository repository, IUserContext userContext, ILogger<EmpresaService> logger)
        : base(userContext)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ResultadoOperacao<IEnumerable<EmpresaDto>>> ListarAsync(CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<IEnumerable<EmpresaDto>>.Falha(auth.Mensagem);

        var dados = await _repository.ListarAsync(ct);
        return ResultadoOperacao<IEnumerable<EmpresaDto>>.Ok(dados.Select(x => x.ToDto()));
    }

    public async Task<ResultadoOperacao<EmpresaDto?>> ObterPorIdAsync(Guid empresaId, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<EmpresaDto?>.Falha(auth.Mensagem);
        if (empresaId == Guid.Empty) return ResultadoOperacao<EmpresaDto?>.Falha("EmpresaId inválido.");

        var empresa = await _repository.ObterPorIdAsync(empresaId, ct);
        return ResultadoOperacao<EmpresaDto?>.Ok(empresa?.ToDto());
    }

    public async Task<ResultadoOperacao<EmpresaDto>> AdicionarAsync(EmpresaDto dto, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<EmpresaDto>.Falha(auth.Mensagem);

        Normalizar(dto);
        if (string.IsNullOrWhiteSpace(dto.RazaoSocial)) return ResultadoOperacao<EmpresaDto>.Falha("Informe a razão social.");
        if (string.IsNullOrWhiteSpace(dto.Cnpj)) return ResultadoOperacao<EmpresaDto>.Falha("Informe o CNPJ.");

        dto.EmpresaId = dto.EmpresaId == Guid.Empty ? Guid.NewGuid() : dto.EmpresaId;
        dto.Ativo = true;
        dto.DataCadastro = dto.DataCadastro == default ? DateTime.UtcNow : dto.DataCadastro;

        var empresa = dto.ToEntity();
        var ok = await _repository.AdicionarAsync(empresa, ct);
        return ok ? ResultadoOperacao<EmpresaDto>.Ok(dto, "Empresa criada com sucesso.") : ResultadoOperacao<EmpresaDto>.Falha("Falha ao criar empresa.");
    }

    public async Task<ResultadoOperacao> AtualizarAsync(Guid empresaId, EmpresaDto dto, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (empresaId == Guid.Empty) return ResultadoOperacao.Falha("EmpresaId inválido.");

        Normalizar(dto);
        dto.EmpresaId = empresaId;
        if (string.IsNullOrWhiteSpace(dto.RazaoSocial)) return ResultadoOperacao.Falha("Informe a razão social.");
        if (string.IsNullOrWhiteSpace(dto.Cnpj)) return ResultadoOperacao.Falha("Informe o CNPJ.");

        var ok = await _repository.AtualizarAsync(dto.ToEntity(), ct);
        if (!ok) _logger.LogWarning("Atualização de empresa não afetou registros. EmpresaId: {EmpresaId}", empresaId);
        return ok ? ResultadoOperacao.Ok("Empresa atualizada com sucesso.") : ResultadoOperacao.Falha("Empresa não encontrada.");
    }

    public async Task<ResultadoOperacao> InativarAsync(Guid empresaId, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (empresaId == Guid.Empty) return ResultadoOperacao.Falha("EmpresaId inválido.");

        var ok = await _repository.InativarAsync(empresaId, ct);
        return ok ? ResultadoOperacao.Ok("Empresa inativada com sucesso.") : ResultadoOperacao.Falha("Empresa não encontrada.");
    }

    private static void Normalizar(EmpresaDto dto)
    {
        dto.RazaoSocial = Normalization.TrimOrEmpty(dto.RazaoSocial);
        dto.NomeFantasia = Normalization.TrimOrEmpty(dto.NomeFantasia);
        dto.Cnpj = Normalization.OnlyDigits(dto.Cnpj);
        dto.Email = Normalization.LowerTrimOrEmpty(dto.Email);
        dto.ContatoEmail = Normalization.LowerTrimOrEmpty(dto.ContatoEmail);
        dto.Cep = Normalization.OnlyDigits(dto.Cep);
        dto.TelefoneDdd = Normalization.OnlyDigits(dto.TelefoneDdd);
        dto.TelefoneNumero = Normalization.OnlyDigits(dto.TelefoneNumero);
        if (dto.EmpresaTipo == 0) dto.EmpresaTipo = 2;
    }
}
