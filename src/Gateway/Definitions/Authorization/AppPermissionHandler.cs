using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Gateway.Definitions.Authorization;

/// <summary>
/// Permission handler for custom authorization implementations
/// </summary>
public class AppPermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity is null)
        {
            return Task.CompletedTask;
        }

        var identity = (ClaimsIdentity)context.User.Identity;

        var permissionClaim = identity.Claims.FirstOrDefault(claim => claim.Type == "permissions");

        if (permissionClaim is null || !permissionClaim.Value.Split(",").Contains(requirement.PermissionName))
        {
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}