using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.UserStore;

public class ApplicationUserStore : IApplicationUserStore
{
    private readonly AppDbContext _context;

    public ApplicationUserStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null || user.UserProfile == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        // check if there are links between AppPermissions and UserProfile
        user.UserProfile.Permissions?.ForEach(p => p.UserProfile = user.UserProfile);



        await _context.Users.AddAsync(user, cancellationToken);
        var quantitySavedEntites = await _context.SaveChangesAsync(cancellationToken);

        return quantitySavedEntites > 0 ? IdentityResult.Success : IdentityResult.Failed();
    }
}