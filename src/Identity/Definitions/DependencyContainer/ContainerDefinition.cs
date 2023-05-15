using Calabonga.AspNetCore.AppDefinitions;
using Identity.Application.Services.Account;
using Identity.Application.Services.EventProcessing;
using Identity.Application.Services.PasswordValidator;
using Identity.AsyncDataServices;
using Identity.AsyncDataServices.MessageBusClients;
using Identity.Definitions.OpenIddict;
using Identity.Infrastructure.Managers.RoleManager;
using Identity.Infrastructure.Managers.UserManager;
using Identity.Infrastructure.Managers.UserRoleStore;
using Identity.Infrastructure.UserStore;

namespace Identity.Definitions.DependencyContainer;

/// <summary>
/// Dependency container definition
/// </summary>
public class ContainerDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddTransient<IApplicationUserStore, ApplicationUserStore>();
        services.AddTransient<IApplicationUserManager, ApplicationUserManager>();
        services.AddTransient<IApplicationRoleManager, ApplicationRoleManager>();
        services.AddTransient<IApplicationUserRoleStore, ApplicationUserRoleStore>();

        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IPasswordValidator, PasswordValidator>();
        services.AddTransient<ApplicationUserClaimsPrincipalFactory>();

        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddHostedService<MessageBusSubscriber>();

        services.AddSingleton<IMessageBusClient, MessageBusClient>();

    }
}