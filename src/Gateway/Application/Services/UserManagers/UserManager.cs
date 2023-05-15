using Gateway.Domain;
using Gateway.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Application.Services.UserManagers;

public class UserManager : IUserManager
{
    private readonly AppDbContext _context;

    public UserManager(AppDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<int> UpdateUserInfo(User user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _context.Users.Update(user);

        return await _context.SaveChangesAsync(cancellationToken);
    }
}