using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Application.Services.FreeScooter;
using Gateway.Application.Services.UserManagers;
using Gateway.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Gateway.Endpoints.FreeScooters;

public class FreeScootersDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapGet("/freescooters", GetFreeScooters).ExcludeFromDescription();
    }
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpClient<IFreeScooterService, FreeScooterService>(client =>
        {
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HttpFreeScooter")!);
        });
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
            Policy = "customer:api")]
    private async Task<IResult> GetFreeScooters(
            HttpContext context,
            [FromServices] IUserManager userManager, 
            [FromServices] IFreeScooterService freeScooterService)
    {
        var id = context.User.GetId();
        
        var user = await userManager.GetUser(id);
        if (user is null || user.Email is null || user.Name is null)
        {
            return Results.Forbid();
        }

        Console.WriteLine($"--> User with id: {id} has requested free scooters. {DateTime.Now}");

        return await freeScooterService.GetFreeScooters();
    }
}