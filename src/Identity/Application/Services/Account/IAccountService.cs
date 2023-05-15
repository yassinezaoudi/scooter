using OpenIddict.Abstractions;

namespace Identity.Application.Services.Account;

public interface IAccountService
{
    Task<IResult> GetTokenAsync(string phoneNumber, string password, OpenIddictRequest request,
        CancellationToken cancellationToken = default(CancellationToken));
}