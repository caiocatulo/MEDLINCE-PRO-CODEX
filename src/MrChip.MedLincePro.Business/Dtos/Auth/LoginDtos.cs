using MrChip.MedLincePro.Business.Dtos.Adm;

namespace MrChip.MedLincePro.Business.Dtos.Auth;

public sealed class LoginRequestDto
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public sealed class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UsuarioDto Usuario { get; set; } = new();
    public BureauDto Bureau { get; set; } = new();
    public EmpresaDto Empresa { get; set; } = new();
}

public sealed class AuthContextDto
{
    public UsuarioDto Usuario { get; set; } = new();
    public BureauDto Bureau { get; set; } = new();
    public EmpresaDto Empresa { get; set; } = new();
    public List<AuthClaimDto> Claims { get; set; } = new();
}

public sealed class AuthClaimDto
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
