using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Application.Services.EventProcessing;
using Gateway.Application.Services.PasswordService;
using Gateway.Application.Services.ScooterManagers;
using Gateway.Application.Services.UserManagers;
using Gateway.AsyncDataServices;
using Gateway.AsyncDataServices.MessageBusClients;

namespace Gateway.Definitions.DependencyContainer;

/// <summary>
/// Dependency container definition
/// </summary>
public class ContainerDefinition : AppDefinition
{
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        

        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();

        services.AddHostedService<MessageBusSubscriber>();

        services.AddScoped<IUserManager, UserManager>();
    }
}