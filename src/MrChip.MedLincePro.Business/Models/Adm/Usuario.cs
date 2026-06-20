using MrChip.MedLincePro.Business.Models;

namespace MrChip.MedLincePro.Business.Models.Adm;

public sealed class Usuario : Entity
{
    public Usuario()
    {
        UsuarioClaims = new List<UsuarioClaim>();
    }

    public Guid UsuarioId { get; set; }
    public Guid BureauId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailConfirmado { get; set; }
    public bool ReadOnly { get; set; }
    public int Grupo { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? CodigoRecuperaSenha { get; set; }
    public List<UsuarioClaim> UsuarioClaims { get; set; }
}

public sealed class UsuarioClaim
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}
