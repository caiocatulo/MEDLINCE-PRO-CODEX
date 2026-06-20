using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Adm;

public sealed class ProfissionalService : ServiceBase, IProfissionalService
{
    private readonly IProfissionalRepository _repository;

    public ProfissionalService(IProfissionalRepository repository, IUserContext userContext)
        : base(userContext)
    {
        _repository = repository;
    }

    public async Task<ResultadoOperacao<IEnumerable<ProfissionalDto>>> ListarAsync(Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<IEnumerable<ProfissionalDto>>.Falha(auth.Mensagem);

        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<IEnumerable<ProfissionalDto>>.Falha("Empresa inválida.");

        var dados = await _repository.ListarPorEmpresaAsync(empresa, ct);
        return ResultadoOperacao<IEnumerable<ProfissionalDto>>.Ok(dados.Select(x => x.ToDto()));
    }

    public async Task<ResultadoOperacao<ProfissionalDto?>> ObterPorIdAsync(Guid profissionalId, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<ProfissionalDto?>.Falha(auth.Mensagem);
        if (profissionalId == Guid.Empty) return ResultadoOperacao<ProfissionalDto?>.Falha("ProfissionalId inválido.");

        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<ProfissionalDto?>.Falha("Empresa inválida.");

        var profissional = await _repository.ObterPorIdAsync(empresa, profissionalId, ct);
        return ResultadoOperacao<ProfissionalDto?>.Ok(profissional?.ToDto());
    }

    public async Task<ResultadoOperacao<ProfissionalDto>> AdicionarAsync(ProfissionalDto dto, Guid bureauId, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<ProfissionalDto>.Falha(auth.Mensagem);

        Normalizar(dto);
        dto.EmpresaId = dto.EmpresaId == Guid.Empty ? EmpresaIdAutenticada : dto.EmpresaId;
        dto.ProfissionalId = dto.ProfissionalId == Guid.Empty ? Guid.NewGuid() : dto.ProfissionalId;
        dto.Ativo = true;
        dto.DataCadastro = dto.DataCadastro == default ? DateTime.UtcNow : dto.DataCadastro;
        bureauId = bureauId == Guid.Empty ? BureauIdAutenticado : bureauId;

        if (dto.EmpresaId == Guid.Empty) return ResultadoOperacao<ProfissionalDto>.Falha("Empresa inválida.");
        if (bureauId == Guid.Empty) return ResultadoOperacao<ProfissionalDto>.Falha("Bureau inválido.");
        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao<ProfissionalDto>.Falha("Informe o nome do profissional.");
        if (string.IsNullOrWhiteSpace(dto.Registro)) return ResultadoOperacao<ProfissionalDto>.Falha("Informe o registro do profissional.");

        var ok = await _repository.AdicionarAsync(dto.ToEntity(), bureauId, ct);
        return ok ? ResultadoOperacao<ProfissionalDto>.Ok(dto, "Profissional criado com sucesso.") : ResultadoOperacao<ProfissionalDto>.Falha("Falha ao criar profissional. Verifique CPF/registro duplicado.");
    }

    public async Task<ResultadoOperacao> AtualizarAsync(Guid profissionalId, ProfissionalDto dto, Guid bureauId, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (profissionalId == Guid.Empty) return ResultadoOperacao.Falha("ProfissionalId inválido.");

        Normalizar(dto);
        dto.ProfissionalId = profissionalId;
        dto.EmpresaId = dto.EmpresaId == Guid.Empty ? EmpresaIdAutenticada : dto.EmpresaId;
        bureauId = bureauId == Guid.Empty ? BureauIdAutenticado : bureauId;

        if (dto.EmpresaId == Guid.Empty) return ResultadoOperacao.Falha("Empresa inválida.");
        if (bureauId == Guid.Empty) return ResultadoOperacao.Falha("Bureau inválido.");
        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao.Falha("Informe o nome do profissional.");

        var ok = await _repository.AtualizarAsync(dto.ToEntity(), bureauId, ct);
        return ok ? ResultadoOperacao.Ok("Profissional atualizado com sucesso.") : ResultadoOperacao.Falha("Profissional não encontrado.");
    }

    public async Task<ResultadoOperacao> InativarAsync(Guid profissionalId, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (profissionalId == Guid.Empty) return ResultadoOperacao.Falha("ProfissionalId inválido.");

        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao.Falha("Empresa inválida.");

        var ok = await _repository.InativarAsync(empresa, profissionalId, ct);
        return ok ? ResultadoOperacao.Ok("Profissional inativado com sucesso.") : ResultadoOperacao.Falha("Profissional não encontrado.");
    }

    private static void Normalizar(ProfissionalDto dto)
    {
        dto.Nome = Normalization.TrimOrEmpty(dto.Nome);
        dto.Registro = Normalization.TrimOrEmpty(dto.Registro);
        dto.Cpf = Normalization.NullIfWhiteSpace(Normalization.OnlyDigits(dto.Cpf));
        dto.Email = Normalization.NullIfWhiteSpace(Normalization.LowerTrimOrEmpty(dto.Email));
        dto.Telefone = Normalization.NullIfWhiteSpace(Normalization.OnlyDigits(dto.Telefone));
        dto.Cep = Normalization.OnlyDigits(dto.Cep);
        dto.EnderecoComplemento = Normalization.NullIfWhiteSpace(dto.EnderecoComplemento);
        dto.CodigoGrauDeParticipacao = Normalization.NullIfWhiteSpace(dto.CodigoGrauDeParticipacao);
        dto.BancoCodigo = Normalization.NullIfWhiteSpace(dto.BancoCodigo);
        dto.BancoAgencia = Normalization.NullIfWhiteSpace(dto.BancoAgencia);
        dto.BancoConta = Normalization.NullIfWhiteSpace(dto.BancoConta);
    }
}
