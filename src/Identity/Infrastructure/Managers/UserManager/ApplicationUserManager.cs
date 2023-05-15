using Identity.Infrastructure.Managers.UserRoleStore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Managers.UserManager;

public class ApplicationUserManager : IApplicationUserManager
{
    private readonly IApplicationUserRoleStore _userRoleStore;
    private readonly AppDbContext _context;

    public ApplicationUserManager(IApplicationUserRoleStore userRoleStore, AppDbContext context)
    {
        _userRoleStore = userRoleStore;
        _context = context;
    }

    public async Task<IdentityResult> AddToRoleAsync(User user, string roleName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        var normalizedRole = roleName.ToUpper();
        if (await _userRoleStore.IsInRoleAsync(user, normalizedRole, cancellationToken))
        {
            return IdentityResult.Failed(new IdentityError { Description = "User already in role" });
        }

        await _userRoleStore.AddToRoleAsync(user, normalizedRole, cancellationToken);

        _context.Users.Update(user);


        // если прошло успешно -> вернуть success
        return await _context.SaveChangesAsync(cancellationToken) > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }


        await _context.Users.AddAsync(user, cancellationToken);

        // если прошло успешно -> вернуть success
        return await _context.SaveChangesAsync(cancellationToken) > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }

    public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _context.Users.Include(user => user.UserProfile)
            .ThenInclude(userProfile => userProfile.Permissions)
            .FirstOrDefaultAsync(user => user.PhoneNumber == phoneNumber, cancellationToken);

        if (user == null)
        {
            return null;
        }

        System.Console.WriteLine($"$--> {user.UserProfile.Id}");

        //var permissions = _context.Permissions.Where(p => p.UserProfile == user.UserProfile).ToList();
        System.Console.WriteLine($"$--> {user.UserProfile.Permissions.Count}");
        return user;
    }
}