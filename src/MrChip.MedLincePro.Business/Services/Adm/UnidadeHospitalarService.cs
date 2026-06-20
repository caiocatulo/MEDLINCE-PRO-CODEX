using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Adm;

public sealed class UnidadeHospitalarService : ServiceBase, IUnidadeHospitalarService
{
    private readonly IUnidadeHospitalarRepository _repository;

    public UnidadeHospitalarService(IUnidadeHospitalarRepository repository, IUserContext userContext)
        : base(userContext)
    {
        _repository = repository;
    }

    public async Task<ResultadoOperacao<IEnumerable<UnidadeHospitalarDto>>> ListarAsync(Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<IEnumerable<UnidadeHospitalarDto>>.Falha(auth.Mensagem);
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<IEnumerable<UnidadeHospitalarDto>>.Falha("Empresa inválida.");

        var dados = await _repository.ListarPorEmpresaAsync(empresa, ct);
        return ResultadoOperacao<IEnumerable<UnidadeHospitalarDto>>.Ok(dados.Select(x => x.ToDto()));
    }

    public async Task<ResultadoOperacao<UnidadeHospitalarDto?>> ObterPorCnesAsync(string codigoCnes, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<UnidadeHospitalarDto?>.Falha(auth.Mensagem);
        codigoCnes = Normalization.TrimOrEmpty(codigoCnes);
        if (string.IsNullOrWhiteSpace(codigoCnes)) return ResultadoOperacao<UnidadeHospitalarDto?>.Falha("Código CNES obrigatório.");

        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<UnidadeHospitalarDto?>.Falha("Empresa inválida.");

        var unidade = await _repository.ObterPorCnesAsync(empresa, codigoCnes, ct);
        return ResultadoOperacao<UnidadeHospitalarDto?>.Ok(unidade?.ToDto());
    }

    public async Task<ResultadoOperacao<UnidadeHospitalarDto>> AdicionarAsync(UnidadeHospitalarDto dto, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<UnidadeHospitalarDto>.Falha(auth.Mensagem);
        Normalizar(dto);
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao<UnidadeHospitalarDto>.Falha("Empresa inválida.");
        if (string.IsNullOrWhiteSpace(dto.CodigoCnes)) return ResultadoOperacao<UnidadeHospitalarDto>.Falha("Código CNES obrigatório.");
        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao<UnidadeHospitalarDto>.Falha("Informe o nome da unidade hospitalar.");
        dto.Ativo = true;
        dto.DataCadastro = dto.DataCadastro == default ? DateTime.UtcNow : dto.DataCadastro;

        var ok = await _repository.AdicionarAsync(empresa, dto.ToEntity(), ct);
        return ok ? ResultadoOperacao<UnidadeHospitalarDto>.Ok(dto, "Unidade hospitalar criada/vinculada com sucesso.") : ResultadoOperacao<UnidadeHospitalarDto>.Falha("Falha ao criar unidade hospitalar.");
    }

    public async Task<ResultadoOperacao> AtualizarAsync(string codigoCnes, UnidadeHospitalarDto dto, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        codigoCnes = Normalization.TrimOrEmpty(codigoCnes);
        if (string.IsNullOrWhiteSpace(codigoCnes)) return ResultadoOperacao.Falha("Código CNES obrigatório.");
        Normalizar(dto);
        dto.CodigoCnes = codigoCnes;
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao.Falha("Empresa inválida.");

        var ok = await _repository.AtualizarAsync(empresa, dto.ToEntity(), ct);
        return ok ? ResultadoOperacao.Ok("Unidade hospitalar atualizada com sucesso.") : ResultadoOperacao.Falha("Unidade hospitalar não encontrada.");
    }

    public async Task<ResultadoOperacao> InativarAsync(string codigoCnes, Guid? empresaId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        codigoCnes = Normalization.TrimOrEmpty(codigoCnes);
        if (string.IsNullOrWhiteSpace(codigoCnes)) return ResultadoOperacao.Falha("Código CNES obrigatório.");
        var empresa = ResolverEmpresaId(empresaId);
        if (empresa == Guid.Empty) return ResultadoOperacao.Falha("Empresa inválida.");

        var ok = await _repository.InativarVinculoAsync(empresa, codigoCnes, ct);
        return ok ? ResultadoOperacao.Ok("Vínculo da unidade hospitalar inativado com sucesso.") : ResultadoOperacao.Falha("Unidade hospitalar não encontrada.");
    }

    private static void Normalizar(UnidadeHospitalarDto dto)
    {
        dto.CodigoCnes = Normalization.TrimOrEmpty(dto.CodigoCnes);
        dto.Cnpj = Normalization.OnlyDigits(dto.Cnpj);
        dto.Nome = Normalization.TrimOrEmpty(dto.Nome);
        dto.NomeReduzido = Normalization.TrimOrEmpty(dto.NomeReduzido);
        dto.ContatoEmail = Normalization.NullIfWhiteSpace(Normalization.LowerTrimOrEmpty(dto.ContatoEmail));
        dto.ContatoNome = Normalization.NullIfWhiteSpace(dto.ContatoNome);
        dto.Token = Normalization.NullIfWhiteSpace(dto.Token);
        dto.Latitude = Normalization.NullIfWhiteSpace(dto.Latitude);
        dto.Longitude = Normalization.NullIfWhiteSpace(dto.Longitude);
        dto.Description = Normalization.NullIfWhiteSpace(dto.Description);
        dto.Uf = Normalization.TrimOrEmpty(dto.Uf).ToUpperInvariant();
    }
}
