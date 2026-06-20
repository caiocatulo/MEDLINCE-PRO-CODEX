using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MrChip.MedLincePro.Api.Configuration;
using MrChip.MedLincePro.Business.Dtos.Auth;

namespace MrChip.MedLincePro.Api.Security;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public JwtTokenResult GerarToken(AuthContextDto context)
    {
        if (string.IsNullOrWhiteSpace(_settings.Key) || _settings.Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key deve possuir no mínimo 32 caracteres e deve ser configurada por ambiente/Key Vault em produção.");

        var expiraEm = DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes <= 0 ? 480 : _settings.ExpirationMinutes);
        var claims = CriarClaims(context);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiraEm,
            signingCredentials: credentials);

        return new JwtTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiraEmUtc = expiraEm
        };
    }

    private static List<Claim> CriarClaims(AuthContextDto context)
    {
        var usuario = context.Usuario;
        var bureau = context.Bureau;
        var empresa = context.Empresa;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new(ClaimTypes.Name, usuario.Login ?? string.Empty),
            new(ClaimTypes.Email, usuario.Email ?? string.Empty),
            new("Login", usuario.Login ?? string.Empty),
            new("Nome", usuario.Nome ?? string.Empty),
            new("UsuarioId", usuario.UsuarioId.ToString()),
            new("UsuarioNome", usuario.Nome ?? string.Empty),
            new("BureauId", usuario.BureauId.ToString()),
            new("EmpresaId", bureau.EmpresaId.ToString()),
            new("NivelAcesso", usuario.Grupo.ToString()),
            new("AppPlantao", empresa.AppPlantao.ToString()),
            new("TrabalhaLoteGrupos", empresa.TrabalhaLoteGrupos.ToString()),
            new("BureauAdm", bureau.Adm.ToString())
        };

        foreach (var dbClaim in context.Claims.Where(c => !string.IsNullOrWhiteSpace(c.Type)))
            claims.Add(new Claim(dbClaim.Type, dbClaim.Value ?? string.Empty));

        return claims;
    }
}
