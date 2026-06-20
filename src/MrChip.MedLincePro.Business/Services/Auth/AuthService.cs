using Microsoft.Extensions.Logging;
using MrChip.MedLincePro.Business.Common;
using MrChip.MedLincePro.Business.Dtos.Auth;
using MrChip.MedLincePro.Business.Interfaces;
using MrChip.MedLincePro.Business.Interfaces.Adm;
using MrChip.MedLincePro.Business.Interfaces.Auth;
using MrChip.MedLincePro.Business.Mapping;
using MrChip.MedLincePro.Business.Services;

namespace MrChip.MedLincePro.Business.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IBureauRepository _bureauRepository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IUsuarioPasswordService _passwordService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IBureauRepository bureauRepository,
        IEmpresaRepository empresaRepository,
        IUsuarioPasswordService passwordService,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _bureauRepository = bureauRepository;
        _empresaRepository = empresaRepository;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<ResultadoOperacao<AuthContextDto>> AutenticarAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var login = Normalization.LowerTrimOrEmpty(dto.Login);

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(dto.Senha))
            return ResultadoOperacao<AuthContextDto>.Falha("Login e senha são obrigatórios.");

        var usuario = await _usuarioRepository.ObterPorLoginParaAutenticacaoAsync(login, ct);
        if (usuario is null || !usuario.Ativo || !_passwordService.VerificarSenhaLegada(dto.Senha, usuario.PasswordHash))
        {
            _logger.LogWarning("Falha de autenticação para login: {Login}", login);
            return ResultadoOperacao<AuthContextDto>.Falha("Credenciais inválidas.");
        }

        usuario.UsuarioClaims = (await _usuarioRepository.ObterClaimsAsync(usuario.UsuarioId, ct)).ToList();
        usuario.PasswordHash = string.Empty;

        var bureau = await _bureauRepository.ObterPorIdAsync(usuario.BureauId, ct);
        if (bureau is null || !bureau.Ativo)
            return ResultadoOperacao<AuthContextDto>.Falha("Bureau do usuário não localizado ou inativo.");

        var empresa = await _empresaRepository.ObterPorIdAsync(bureau.EmpresaId, ct);
        if (empresa is null || !empresa.Ativo)
            return ResultadoOperacao<AuthContextDto>.Falha("Empresa do usuário não localizada ou inativa.");

        var context = new AuthContextDto
        {
            Usuario = usuario.ToDto(),
            Bureau = bureau.ToDto(),
            Empresa = empresa.ToDto(),
            Claims = usuario.UsuarioClaims
                .Where(c => !string.IsNullOrWhiteSpace(c.ClaimType))
                .Select(c => new AuthClaimDto { Type = c.ClaimType, Value = c.ClaimValue ?? string.Empty })
                .ToList()
        };

        return ResultadoOperacao<AuthContextDto>.Ok(context);
    }
}
