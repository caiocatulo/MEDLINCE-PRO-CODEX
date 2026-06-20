namespace MrChip.MedLincePro.Business.Configuration;

public sealed class PasswordCompatibilityOptions
{
    public const string SectionName = "PasswordCompatibility";

    /// <summary>
    /// Chave legada usada apenas para compatibilidade com PasswordHash existente.
    /// Não versionar valor real em appsettings. Definir por variável de ambiente:
    /// PasswordCompatibility__LegacyTripleDesKey.
    /// </summary>
    public string LegacyTripleDesKey { get; set; } = string.Empty;
}
