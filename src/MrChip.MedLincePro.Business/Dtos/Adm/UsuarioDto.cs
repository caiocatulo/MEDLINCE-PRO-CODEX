namespace MrChip.MedLincePro.Business.Dtos.Adm;

public sealed class UsuarioDto
{
    public Guid UsuarioId { get; set; }
    public Guid BureauId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public bool EmailConfirmado { get; set; }
    public bool ReadOnly { get; set; }
    public int Grupo { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? CodigoRecuperaSenha { get; set; }
    public DateTime DataCadastro { get; set; }
    public bool Ativo { get; set; }
    public List<UsuarioClaimDto> UsuarioClaims { get; set; } = new();
}

public sealed class UsuarioCreateDto
{
    public Guid BureauId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public int Grupo { get; set; }
    public string Email { get; set; } = string.Empty;
}

public sealed class UsuarioUpdateDto
{
    public Guid UsuarioId { get; set; }
    public Guid BureauId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string? NovaSenha { get; set; }
    public int Grupo { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? CodigoRecuperaSenha { get; set; }
    public bool Ativo { get; set; }
}

public sealed class UsuarioClaimDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}
