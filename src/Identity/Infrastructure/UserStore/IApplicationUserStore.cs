using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.UserStore;

public interface IApplicationUserStore
{
    public Task<IdentityResult> CreateAsync(User user,
        CancellationToken cancellationToken = default);
}