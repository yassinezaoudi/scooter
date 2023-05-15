using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Managers.RoleManager;

public class ApplicationRoleManager : IApplicationRoleManager
{
    private readonly AppDbContext _context;

    public ApplicationRoleManager(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IdentityResult> CreateAsync(Role role,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }

        // check if already there is a role in repository
        var roleResult = await GetRoleByNormalizedNameAsync(role.NormalizedName, cancellationToken);
        if (roleResult is not null)
        {
            return IdentityResult.Failed(
                new IdentityError { Description = "This role is already exists in a database" });
        }

        await _context.Roles.AddAsync(role, cancellationToken);
        return await _context.SaveChangesAsync(cancellationToken) > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<Role?> GetRoleByNormalizedNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _context.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName, cancellationToken);
    }
}