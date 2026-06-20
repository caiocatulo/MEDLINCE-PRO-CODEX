using Microsoft.AspNetCore.Authorization;

namespace MrChip.MedLincePro.Api.Security;

public sealed class ClaimRequirement : IAuthorizationRequirement
{
    public ClaimRequirement(string claimType, IReadOnlyCollection<string> allowedValues)
    {
        ClaimType = claimType;
        AllowedValues = allowedValues;
    }

    public string ClaimType { get; }
    public IReadOnlyCollection<string> AllowedValues { get; }
}
