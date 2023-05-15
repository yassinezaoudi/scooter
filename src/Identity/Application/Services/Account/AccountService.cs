using System.Security.Claims;
using AutoMapper;
using Identity.Application.Services.PasswordValidator;
using Identity.AsyncDataServices.MessageBusClients;
using Identity.Definitions.OpenIddict;
using Identity.Domain.Base.AppData;
using Identity.Dtos;
using Identity.Infrastructure;
using Identity.Infrastructure.Managers.RoleManager;
using Identity.Infrastructure.Managers.UserManager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly IApplicationUserManager _userManager;
    private readonly IPasswordValidator _passwordValidator;
    private readonly ApplicationUserClaimsPrincipalFactory _claimsFactory;
    private readonly AppDbContext _context;
    private readonly IApplicationRoleManager _roleManager;
    private readonly IMapper _mapper;
    private readonly IMessageBusClient _busClient;

    public AccountService(IApplicationUserManager userManager, IApplicationRoleManager roleManager,
        IPasswordValidator passwordValidator,
        ApplicationUserClaimsPrincipalFactory claimsFactory, AppDbContext context, IMapper mapper, IMessageBusClient busClient)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _passwordValidator = passwordValidator;
        _claimsFactory = claimsFactory;
        _context = context;
        _mapper = mapper;
        _busClient = busClient;
    }

    public async Task<IResult> GetTokenAsync(string phoneNumber, string password, OpenIddictRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.GetUserByPhoneNumberAsync(phoneNumber, cancellationToken);

        var passwordVerificationResult =
            await _passwordValidator.ValidateOneTimePassword(phoneNumber, password, once: true);

        if (passwordVerificationResult is PasswordVerificationResult.Failed)
        {
            return Results.Problem("The specified user can not be authorized.");
        }

        ClaimsPrincipal? principal;
        if (user is null)
        {
            principal = await RegisterAsync(phoneNumber, cancellationToken);

            if (principal is null)
            {
                throw new NullReferenceException($"Registration result is null, {phoneNumber}");
            }
        }
        else
        {
            principal = await GetPrincipalByPhoneNumberAsync(phoneNumber);
        }

        principal.SetScopes(request.GetScopes());

        return Results.SignIn(principal, new AuthenticationProperties(),
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Returns <see cref="ApplicationUser"/> instance after successful registration
    /// </summary>
    /// <param name="model"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ClaimsPrincipal?> RegisterAsync(string phoneNumber, CancellationToken cancellationToken)
    {
        var user = new User
        {
            PhoneNumber = phoneNumber
        };

        //await using var transaction = await _unitOfWork.BeginTransactionAsync();
        var result = await _userManager.CreateAsync(user, cancellationToken);
        const string role = AppData.UserRoleName;

        if (result.Succeeded)
        {
            if (await _roleManager.GetRoleByNormalizedNameAsync(role.ToUpper(), cancellationToken) == null)
            {
                //return Results.NotFound(AppData.Exceptions.UserNotFoundException);
                return null;
            }

            await _userManager.AddToRoleAsync(user, role, cancellationToken);

            var profile = new UserProfile
            {
                User = user
            };

            // Creating list of permittions

            var permissions = new List<AppPermission>
            {
                new()
                {
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    CreatedBy = "Registration",
                    PolicyName = "customer:api",
                    Description = "Access policy for using api"
                }
            };
            profile.Permissions = permissions;

            await _context.UserProfiles.AddAsync(profile, cancellationToken);
            var quantitySaveEntities = await _context.SaveChangesAsync(cancellationToken);

            // если все хорошо сохранилось
            if (quantitySaveEntities > 0)
            {
                System.Console.WriteLine("--> User was created!" + user.Id + "|" + user.PhoneNumber);
                var userRegisteredDto = _mapper.Map<PublishUserDto>(user);
                _busClient.PublishNewUser(userRegisteredDto);

                var principal = await _claimsFactory.CreateAsync(user);
                return principal;
            }

            return null;
        }

        return null;
    }

    /// <summary>
    /// Returns ClaimPrincipal by user phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public async Task<ClaimsPrincipal> GetPrincipalByPhoneNumberAsync(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentNullException();
        }

        var user = await _userManager.GetUserByPhoneNumberAsync(phoneNumber);
        if (user == null)
        {
            throw new Exception("User has not been found");
        }

        var defaultClaims = await _claimsFactory.CreateAsync(user);
        return defaultClaims;
    }
}