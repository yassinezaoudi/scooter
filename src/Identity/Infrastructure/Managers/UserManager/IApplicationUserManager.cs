using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Managers.UserManager;

public interface IApplicationUserManager
{
    Task<IdentityResult> AddToRoleAsync(User user, string roleName,
        CancellationToken cancellationToken = default);
    Task<IdentityResult> CreateAsync(User user,
        CancellationToken cancellationToken = default);

    Task<User?> GetUserByPhoneNumberAsync(string phoneNumber,
        CancellationToken cancellationToken = default);
}