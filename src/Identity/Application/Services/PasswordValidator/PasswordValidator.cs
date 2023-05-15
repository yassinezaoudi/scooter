using Identity.Domain;
using Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Services.PasswordValidator;

public class PasswordValidator : IPasswordValidator
{
    private readonly AppDbContext _context;

    public PasswordValidator(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordVerificationResult> ValidateOneTimePassword(string phoneNumber, string password,
        bool once = true, CancellationToken cancellationToken = default)
    {
        var dateTimeNow = DateTime.UtcNow;

        Console.WriteLine(_context.OneTimePasswords.Count());

        var passwordFromDb = await _context.OneTimePasswords.FirstOrDefaultAsync(
            predicate: p =>
                p.PhoneNumber == phoneNumber
                && p.Code == password
                && p.IsActive
                && p.ExpiresAt > dateTimeNow
                && p.NotBefore < dateTimeNow, cancellationToken);

        if (passwordFromDb is not null)
        {
            if (once)
            {
                //await using var transaction = await _unitOfWork.BeginTransactionAsync();
                passwordFromDb.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
                //await transaction.CommitAsync();
            }

            return PasswordVerificationResult.Success;
        }

        return PasswordVerificationResult.Failed;
    }
}