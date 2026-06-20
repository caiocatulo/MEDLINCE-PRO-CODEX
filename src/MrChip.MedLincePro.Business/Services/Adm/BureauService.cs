using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Adm;

public sealed class BureauService : ServiceBase, IBureauService
{
    private readonly IBureauRepository _repository;
    private readonly ILogger<BureauService> _logger;

    public BureauService(IBureauRepository repository, IUserContext userContext, ILogger<BureauService> logger)
        : base(userContext)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ResultadoOperacao<IEnumerable<BureauDto>>> ListarAsync(Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<IEnumerable<BureauDto>>.Falha(auth.Mensagem);

        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<IEnumerable<BureauDto>>.Falha("Empresa inválida.");

        var dados = await _repository.ListarPorEmpresaAsync(empresa, ct);
        return ResultadoOperacao<IEnumerable<BureauDto>>.Ok(dados.Select(x => x.ToDto()));
    }

    public async Task<ResultadoOperacao<BureauDto?>> ObterPorIdAsync(Guid bureauId, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<BureauDto?>.Falha(auth.Mensagem);
        if (bureauId == Guid.Empty) return ResultadoOperacao<BureauDto?>.Falha("BureauId inválido.");

        var bureau = await _repository.ObterPorIdAsync(bureauId, ct);
        return ResultadoOperacao<BureauDto?>.Ok(bureau?.ToDto());
    }

    public async Task<ResultadoOperacao<BureauDto>> AdicionarAsync(BureauDto dto, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<BureauDto>.Falha(auth.Mensagem);

        dto.EmpresaId = ResolverEmpresaId(dto.EmpresaId == Guid.Empty ? null : dto.EmpresaId);
        dto.BureauId = dto.BureauId == Guid.Empty ? Guid.NewGuid() : dto.BureauId;
        dto.Ativo = true;
        dto.DataCadastro = dto.DataCadastro == default ? DateTime.UtcNow : dto.DataCadastro;
        dto.ApiAcesso = dto.ApiAcesso == default ? DateTime.UtcNow : dto.ApiAcesso;
        Normalizar(dto);

        if (dto.EmpresaId == Guid.Empty) return ResultadoOperacao<BureauDto>.Falha("Empresa inválida.");
        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao<BureauDto>.Falha("Informe o nome do bureau.");
        if (string.IsNullOrWhiteSpace(dto.CpfCnpj)) return ResultadoOperacao<BureauDto>.Falha("Informe o CPF/CNPJ do bureau.");

        var ok = await _repository.AdicionarAsync(dto.ToEntity(), ct);
        return ok ? ResultadoOperacao<BureauDto>.Ok(dto, "Bureau criado com sucesso.") : ResultadoOperacao<BureauDto>.Falha("Falha ao criar bureau.");
    }

    public async Task<ResultadoOperacao> AtualizarAsync(Guid bureauId, BureauDto dto, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (bureauId == Guid.Empty) return ResultadoOperacao.Falha("BureauId inválido.");

        dto.BureauId = bureauId;
        dto.EmpresaId = ResolverEmpresaId(dto.EmpresaId == Guid.Empty ? null : dto.EmpresaId);
        Normalizar(dto);

        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao.Falha("Informe o nome do bureau.");

        var ok = await _repository.AtualizarAsync(dto.ToEntity(), ct);
        if (!ok) _logger.LogWarning("Atualização de bureau não afetou registros. BureauId: {BureauId}", bureauId);
        return ok ? ResultadoOperacao.Ok("Bureau atualizado com sucesso.") : ResultadoOperacao.Falha("Bureau não encontrado.");
    }

    public async Task<ResultadoOperacao> InativarAsync(Guid bureauId, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (bureauId == Guid.Empty) return ResultadoOperacao.Falha("BureauId inválido.");
        if (EmpresaIdAutenticada == Guid.Empty) return ResultadoOperacao.Falha("Empresa do usuário autenticado não identificada.");

        var ok = await _repository.InativarAsync(bureauId, EmpresaIdAutenticada, ct);
        return ok ? ResultadoOperacao.Ok("Bureau inativado com sucesso.") : ResultadoOperacao.Falha("Bureau não encontrado.");
    }

    private static void Normalizar(BureauDto dto)
    {
        dto.Nome = Normalization.TrimOrEmpty(dto.Nome);
        dto.NomeResumido = Normalization.TrimOrEmpty(dto.NomeResumido);
        dto.CpfCnpj = Normalization.OnlyDigits(dto.CpfCnpj);
        dto.Email = Normalization.LowerTrimOrEmpty(dto.Email);
        dto.Cep = Normalization.OnlyDigits(dto.Cep);
        dto.Adm = string.IsNullOrWhiteSpace(dto.Adm) ? "N" : dto.Adm.Trim().ToUpperInvariant();
    }
}
