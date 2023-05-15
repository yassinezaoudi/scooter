using Identity.Infrastructure.Managers.RoleManager;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Managers.UserRoleStore;

public class ApplicationUserRoleStore : IApplicationUserRoleStore
{
    private readonly AppDbContext _context;
    private readonly IApplicationRoleManager _roleManager;

    public ApplicationUserRoleStore(AppDbContext context, IApplicationRoleManager roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    public async Task<bool> IsInRoleAsync(User user, string normalizedRole,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var role = await _roleManager.GetRoleByNormalizedNameAsync(normalizedRole, cancellationToken);
        
        if (role is null)
        {
            return false;
        }


        // проверка на наличие роли у пользователя
        return await _context.UserRoles.AnyAsync(ur => ur.Role == role && ur.User == user);
    }

    public async Task AddToRoleAsync(User user, string normalizedRole,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();


        var role = await _roleManager.GetRoleByNormalizedNameAsync(normalizedRole, cancellationToken);
        if (role is null)
        {
            return;
        }

        var userRole = new UserRole
        {
            User = user,
            Role = role
        };

        await _context.UserRoles.AddAsync(userRole, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}