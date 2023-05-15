﻿using System.Text;
using System.Security.Claims;
using Identity.Infrastructure;
using Identity.Infrastructure.Managers.RoleManager;
using Identity.Infrastructure.Managers.UserManager;
using OpenIddict.Abstractions;

namespace Identity.Definitions.OpenIddict;

/// <summary>
/// User Claims Principal Factory override from Microsoft Identity framework
/// </summary>
public class ApplicationUserClaimsPrincipalFactory
{
    private readonly IApplicationUserManager _userManager;
    private readonly IApplicationRoleManager _roleManager;

    public ApplicationUserClaimsPrincipalFactory(
        IApplicationUserManager userManager,
        IApplicationRoleManager roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Creates a <see cref="T:System.Security.Claims.ClaimsPrincipal" /> from an user asynchronously.
    /// </summary>
    /// <param name="user">The user to create a <see cref="T:System.Security.Claims.ClaimsPrincipal" /> from.</param>
    /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous creation operation, containing the created <see cref="T:System.Security.Claims.ClaimsPrincipal" />.</returns>
    public async Task<ClaimsPrincipal> CreateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var id = await GenerateClaimsAsync(user);
        var principal = new ClaimsPrincipal(id);

        if (user.UserProfile?.Permissions != null)
        {
            var policies = user.UserProfile.Permissions.Select(p => p.PolicyName);
            StringBuilder permissionsBuilder = new StringBuilder();
            foreach (var policy in policies)
            {
                if (permissionsBuilder.Length == 0)
                {
                    permissionsBuilder.Insert(0, $"{policy}");
                }
                else
                {
                    permissionsBuilder.Insert(permissionsBuilder.Length, $",{policy}");
                }
            }
            ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim("permissions",
                        permissionsBuilder.ToString()));
            // var permissions = user.UserProfile.Permissions.ToList();
            // if (permissions.Any())
            // {
            //     permissions.ForEach(x =>
            //     {

            //         ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(x.PolicyName,
            //             nameof(x.PolicyName).ToLower()));
            //         System.Console.WriteLine($"$--> ApplicationUserClaimsPrincipal: {x.PolicyName}");
            //     });
            // }
        }



        //// For this sample, just include all claims in all token types.
        //// In reality, claims' destinations would probably differ by token type and depending on the scopes requested.
        //// In our case (demo) we're using OpenIddictConstants.Destinations.AccessToken and OpenIddictConstants.Destinations.IdentityToken
        foreach (var principalClaim in principal.Claims)
        {
            principalClaim.SetDestinations(OpenIddictConstants.Destinations.AccessToken,
                OpenIddictConstants.Destinations.IdentityToken);
        }

        return principal;
    }

    /// <summary>
    /// Generate the claims for a user.
    /// </summary>
    /// <param name="user">The user to create a <see cref="ClaimsIdentity"/> from.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous creation operation, containing the created <see cref="ClaimsIdentity"/>.</returns>
    private async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var userId = user.Id.ToString();
        // REVIEW: Used to match Application scheme
        var id = new ClaimsIdentity("Identity.Application");
        id.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, userId));
        return id;
    }
}