namespace MrChip.MedLincePro.Business.Services;

internal static class Normalization
{
    public static string TrimOrEmpty(string? value) => (value ?? string.Empty).Trim();

    public static string LowerTrimOrEmpty(string? value) => TrimOrEmpty(value).ToLowerInvariant();

    public static string? NullIfWhiteSpace(string? value)
    {
        var normalized = TrimOrEmpty(value);
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    public static string OnlyDigits(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return new string(value.Where(char.IsDigit).ToArray());
    }
}
