using Calabonga.AspNetCore.AppDefinitions;
using Identity.HostedServices;
using Identity.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using OpenIddict.Abstractions;

namespace Identity.Definitions.OpenIddict;

public class OpenIddictDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default entities.
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>()
                    .ReplaceDefaultEntities<Guid>();
            })

            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                options
                    .AllowPasswordFlow()
                    .AllowClientCredentialsFlow()
                    .AllowRefreshTokenFlow();

                options.SetIssuer(new Uri(Environment.GetEnvironmentVariable("Issuer")!));

                // Enable the token endpoint.
                options
                    .SetLogoutEndpointUris("/connect/logout")
                    .SetTokenEndpointUris("/connect/token");

                // Encryption and signing of tokens
                options
                    .AddEphemeralEncryptionKey() // only for Developing mode
                    .AddEphemeralSigningKey() // only for Developing mode
                    .DisableAccessTokenEncryption(); // only for Developing mode

                // Mark the "email", "profile" and "roles" scopes as supported scopes.
                options.RegisterScopes(
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.Roles,
                    "api");

                // Register the signing and encryption credentials.
                options
                    .AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                // Register the ASP.NET Core host and configure the ASP.NET Core options.
                options
                    .UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough()
                    .DisableTransportSecurityRequirement();
            })
            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {
                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        // Register the worker responsible for seeding the database.
        // Note: in a real world application, this step should be part of a setup script.
        services.AddHostedService<OpenIddictWorker>();

        services.AddDataProtection()
            .PersistKeysToDbContext<AppDbContext>();
    }
}