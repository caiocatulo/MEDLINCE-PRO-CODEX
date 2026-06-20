using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MrChip.MedLincePro.Business.Configuration;
using MrChip.MedLincePro.Business.Interfaces;

namespace MrChip.MedLincePro.Business.Services.Security;

public sealed class LegacyPasswordService : IUsuarioPasswordService
{
    private readonly PasswordCompatibilityOptions _options;

    public LegacyPasswordService(IOptions<PasswordCompatibilityOptions> options)
    {
        _options = options.Value;
    }

    public string HashParaCompatibilidadeLegada(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return string.Empty;

        if (string.IsNullOrWhiteSpace(_options.LegacyTripleDesKey) || IsPlaceholder(_options.LegacyTripleDesKey))
            throw new InvalidOperationException("Chave legada de senha não configurada. Defina PasswordCompatibility__LegacyTripleDesKey fora do código-fonte.");

        var keyBytes = Encoding.UTF8.GetBytes(_options.LegacyTripleDesKey);
        if (keyBytes.Length is not (16 or 24))
            throw new InvalidOperationException("Chave legada de senha inválida. TripleDES exige 16 ou 24 bytes.");

        using var tripleDes = TripleDES.Create();
        tripleDes.Key = keyBytes;
        tripleDes.Mode = CipherMode.ECB;
        tripleDes.Padding = PaddingMode.PKCS7;

        var data = Encoding.UTF8.GetBytes(senha);
        using var encryptor = tripleDes.CreateEncryptor();
        var result = encryptor.TransformFinalBlock(data, 0, data.Length);

        return Convert.ToBase64String(result);
    }

    private static bool IsPlaceholder(string value) =>
        value.StartsWith('{') && value.EndsWith('}');

    public bool VerificarSenhaLegada(string senhaInformada, string passwordHashPersistido)
    {
        if (string.IsNullOrWhiteSpace(senhaInformada) || string.IsNullOrWhiteSpace(passwordHashPersistido))
            return false;

        var computedHash = HashParaCompatibilidadeLegada(senhaInformada);
        var left = Encoding.UTF8.GetBytes(computedHash);
        var right = Encoding.UTF8.GetBytes(passwordHashPersistido);

        return left.Length == right.Length && CryptographicOperations.FixedTimeEquals(left, right);
    }
}
