using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Adm;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Adm;

public sealed class UsuarioService : ServiceBase, IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly IUsuarioPasswordService _passwordService;

    public UsuarioService(IUsuarioRepository repository, IUsuarioPasswordService passwordService, IUserContext userContext)
        : base(userContext)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<ResultadoOperacao<List<UsuarioDto>>> ListarPorBureauAsync(Guid? bureauId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<List<UsuarioDto>>.Falha(auth.Mensagem);

        var bureau = ResolverBureauId(bureauId);
        if (bureau == Guid.Empty) return ResultadoOperacao<List<UsuarioDto>>.Falha("Bureau inválido.");

        var usuarios = await _repository.ListarPorBureauAsync(bureau, ct);
        return ResultadoOperacao<List<UsuarioDto>>.Ok(usuarios.Select(x => x.ToDto()).ToList());
    }

    public async Task<ResultadoOperacao<UsuarioDto?>> ObterPorIdAsync(Guid usuarioId, Guid? bureauId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return ResultadoOperacao<UsuarioDto?>.Falha(auth.Mensagem);
        if (usuarioId == Guid.Empty) return ResultadoOperacao<UsuarioDto?>.Falha("UsuarioId inválido.");

        var bureau = ResolverBureauId(bureauId);
        if (bureau == Guid.Empty) return ResultadoOperacao<UsuarioDto?>.Falha("Bureau inválido.");

        var usuario = await _repository.ObterPorIdAsync(usuarioId, bureau, ct);
        return ResultadoOperacao<UsuarioDto?>.Ok(usuario?.ToDto());
    }

    public async Task<ResultadoOperacao> CriarAsync(UsuarioCreateDto dto, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;

        Normalizar(dto);
        dto.BureauId = dto.BureauId == Guid.Empty ? BureauIdAutenticado : dto.BureauId;
        if (dto.BureauId == Guid.Empty) return ResultadoOperacao.Falha("Bureau inválido.");
        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao.Falha("Informe o nome do usuário.");
        if (string.IsNullOrWhiteSpace(dto.Login)) return ResultadoOperacao.Falha("Informe o login do usuário.");
        if (string.IsNullOrWhiteSpace(dto.Senha)) return ResultadoOperacao.Falha("Informe a senha do usuário.");
        if (string.IsNullOrWhiteSpace(dto.Email)) return ResultadoOperacao.Falha("Informe o e-mail do usuário.");

        var passwordHash = _passwordService.HashParaCompatibilidadeLegada(dto.Senha);
        var ok = await _repository.CriarAsync(dto, passwordHash, ct);
        return ok ? ResultadoOperacao.Ok("Usuário criado com sucesso.") : ResultadoOperacao.Falha("Falha ao criar usuário. Verifique se login ou e-mail já existem no bureau.");
    }

    public async Task<ResultadoOperacao> AtualizarAsync(Guid usuarioId, UsuarioUpdateDto dto, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (usuarioId == Guid.Empty) return ResultadoOperacao.Falha("UsuarioId inválido.");

        Normalizar(dto);
        dto.UsuarioId = usuarioId;
        dto.BureauId = dto.BureauId == Guid.Empty ? BureauIdAutenticado : dto.BureauId;
        if (dto.BureauId == Guid.Empty) return ResultadoOperacao.Falha("Bureau inválido.");
        if (string.IsNullOrWhiteSpace(dto.Nome)) return ResultadoOperacao.Falha("Informe o nome do usuário.");
        if (string.IsNullOrWhiteSpace(dto.Login)) return ResultadoOperacao.Falha("Informe o login do usuário.");
        if (string.IsNullOrWhiteSpace(dto.Email)) return ResultadoOperacao.Falha("Informe o e-mail do usuário.");

        var novoHash = string.IsNullOrWhiteSpace(dto.NovaSenha) ? null : _passwordService.HashParaCompatibilidadeLegada(dto.NovaSenha);
        var ok = await _repository.AtualizarAsync(dto, novoHash, ct);
        return ok ? ResultadoOperacao.Ok("Usuário atualizado com sucesso.") : ResultadoOperacao.Falha("Usuário não encontrado.");
    }

    public async Task<ResultadoOperacao> InativarAsync(Guid usuarioId, Guid? bureauId = null, CancellationToken ct = default)
    {
        var auth = ValidarAutenticacao();
        if (!auth.Sucesso) return auth;
        if (usuarioId == Guid.Empty) return ResultadoOperacao.Falha("UsuarioId inválido.");

        var bureau = ResolverBureauId(bureauId);
        if (bureau == Guid.Empty) return ResultadoOperacao.Falha("Bureau inválido.");

        var ok = await _repository.InativarAsync(usuarioId, bureau, ct);
        return ok ? ResultadoOperacao.Ok("Usuário inativado com sucesso.") : ResultadoOperacao.Falha("Usuário não encontrado.");
    }

    private static void Normalizar(UsuarioCreateDto dto)
    {
        dto.Nome = Normalization.TrimOrEmpty(dto.Nome);
        dto.Sobrenome = Normalization.TrimOrEmpty(dto.Sobrenome);
        dto.Login = Normalization.LowerTrimOrEmpty(dto.Login);
        dto.Email = Normalization.LowerTrimOrEmpty(dto.Email);
        dto.Grupo = dto.Grupo == 0 ? 3 : dto.Grupo;
    }

    private static void Normalizar(UsuarioUpdateDto dto)
    {
        dto.Nome = Normalization.TrimOrEmpty(dto.Nome);
        dto.Sobrenome = Normalization.TrimOrEmpty(dto.Sobrenome);
        dto.Login = Normalization.LowerTrimOrEmpty(dto.Login);
        dto.Email = Normalization.LowerTrimOrEmpty(dto.Email);
        dto.Grupo = dto.Grupo == 0 ? 3 : dto.Grupo;
    }
}
