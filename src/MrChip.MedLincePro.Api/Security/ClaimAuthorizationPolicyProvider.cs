using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace MrChip.MedLincePro.Api.Security;

public sealed class ClaimAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private const string Prefix = "claim:";

    public ClaimAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public static string BuildPolicyName(string claimType, IReadOnlyCollection<string> allowedValues)
    {
        var values = allowedValues.Count == 0 ? string.Empty : string.Join('|', allowedValues);
        return $"{Prefix}{claimType}:{values}";
    }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            return base.GetPolicyAsync(policyName);

        var payload = policyName[Prefix.Length..];
        var separator = payload.IndexOf(':');
        var claimType = separator >= 0 ? payload[..separator] : payload;
        var rawValues = separator >= 0 ? payload[(separator + 1)..] : string.Empty;
        var values = rawValues.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new ClaimRequirement(claimType, values))
            .Build();

        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}
