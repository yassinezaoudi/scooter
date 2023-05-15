using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Managers.RoleManager;

public interface IApplicationRoleManager
{
    Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role?> GetRoleByNormalizedNameAsync(string roleName, CancellationToken cancellationToken = default);
}