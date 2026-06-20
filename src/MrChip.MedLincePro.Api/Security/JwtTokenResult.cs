namespace MrChip.MedLincePro.Api.Security;

public sealed class JwtTokenResult
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiraEmUtc { get; init; }
}
