using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Services.PasswordValidator;

public interface IPasswordValidator
{
    Task<PasswordVerificationResult> ValidateOneTimePassword(string phoneNumber, string password,
        bool once = true, CancellationToken cancellationToken = default);
}