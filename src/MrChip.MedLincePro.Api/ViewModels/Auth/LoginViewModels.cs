using System.ComponentModel.DataAnnotations;
using MrChip.MedLincePro.Business.Dtos.Adm;

namespace MrChip.MedLincePro.Api.ViewModels.Auth;

public sealed class LoginRequestViewModel
{
    [Required]
    [MaxLength(150)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Senha { get; set; } = string.Empty;
}

public sealed class LoginResponseViewModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiraEmUtc { get; set; }
    public UsuarioDto Usuario { get; set; } = new();
    public BureauDto Bureau { get; set; } = new();
    public EmpresaDto Empresa { get; set; } = new();
}
