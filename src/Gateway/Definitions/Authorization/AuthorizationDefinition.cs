using System.Text.Json;
using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Validation.AspNetCore;

namespace Gateway.Definitions.Authorization;

public class AuthorizationDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {

        // services.AddAuthentication()
        //     .AddJwtBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, options =>
        //     {
        //         options.Authority = Environment.GetEnvironmentVariable("Authority");

        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateAudience = false
        //         };
        //         options.RequireHttpsMetadata = false;
        //     });


        services.AddOpenIddict()
            .AddValidation(options =>
            {
                // Note: the validation handler uses OpenID Connect discovery
                // to retrieve the issuer signing keys used to validate tokens.
                options.SetIssuer(Environment.GetEnvironmentVariable("Authority")!);
                //options.AddAudiences("resource_server_2");
                // Register the encryption credentials. This sample uses a symmetric
                // encryption key that is shared between the server and the Api2 sample
                // (that performs local token validation instead of using introspection).
                //
                // Note: in a real world application, this encryption key should be
                // stored in a safe place (e.g in Azure KeyVault, stored as a secret).
                // options.AddEncryptionKey(new SymmetricSecurityKey(
                //     Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));
                options.UseSystemNetHttp();
                //options.UseLocalServer();
                // Register the ASP.NET Core host.
                options.UseAspNetCore();

            });

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthData.AuthSchemes, policy =>
            {
                policy.RequireAuthenticatedUser();
                //policy.RequireClaim("scope", "api");
            });
        });
        services.AddDataProtection()
        .PersistKeysToDbContext<AppDbContext>();

        services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, AppPermissionHandler>();

    }

    public override void ConfigureApplication(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}