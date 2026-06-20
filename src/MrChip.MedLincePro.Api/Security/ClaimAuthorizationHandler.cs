using Microsoft.AspNetCore.Authorization;

namespace MrChip.MedLincePro.Api.Security;

public sealed class ClaimAuthorizationHandler : AuthorizationHandler<ClaimRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimRequirement requirement)
    {
        var userValues = context.User.FindAll(requirement.ClaimType)
            .SelectMany(c => (c.Value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (requirement.AllowedValues.Count == 0 || requirement.AllowedValues.Any(userValues.Contains))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
