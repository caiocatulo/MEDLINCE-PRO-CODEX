using Microsoft.AspNetCore.Authorization;

namespace MrChip.MedLincePro.Api.Security;

public sealed class ClaimsAuthorizeAttribute : AuthorizeAttribute
{
    public ClaimsAuthorizeAttribute(string claimType, params string[] allowedValues)
    {
        Policy = ClaimAuthorizationPolicyProvider.BuildPolicyName(claimType, allowedValues);
    }
}
