using Calabonga.AspNetCore.AppDefinitions;
using Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Identity.Definitions.DbContext;

/// <summary>
/// ASP.NET Core services registration and configurations
/// </summary>
public class DbContextDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<AppDbContext>(config =>
        {
            var environment = builder.Environment;

            if (environment.IsDevelopment())
            {
                config.UseInMemoryDatabase("DEMO-PURPOSES-ONLY");

            }
            if (environment.IsProduction())
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                
                config.UseNpgsql(connectionString);
            }

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need to replace the default OpenIddict entities.
            config.UseOpenIddict<Guid>();
        });


        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            // configure more options if you need
        });

    }
}