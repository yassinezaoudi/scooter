using AutoMapper;
using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Application.Services.ScooterManagers;
using Gateway.Endpoints.ScooterController.ViewModels;
using Gateway.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Gateway.Endpoints.ScooterController;

public class ScooterControllerDefinition : AppDefinition
{
    private IScooterManager? _scooterManager;

    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpClient<IScooterManager, ScooterManager>(client =>
        {
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HttpScooterController")!);
        });
    }
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapPost("/addscooter", AddScooterAsync).ExcludeFromDescription();
        app.MapPost("/hidescooter", HideScooter).ExcludeFromDescription();
        app.MapPost("/appearscooter", AppearScooter).ExcludeFromDescription();

        _scooterManager = app.Services.GetService<IScooterManager>();
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Policy = "addscooter:api")]
    private async Task<IResult> AddScooterAsync(HttpContext context,
        [FromServices] IMapper mapper, 
        [FromBody] AddScooterViewModel viewModel)
    {
        var id = context.User.GetId();
        Console.WriteLine($"--> User with id: {id} has added scooter {viewModel.Model}," +
                          $" {viewModel.LinkToScooter}");

        return await _scooterManager.AddScooter(viewModel);
    }
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Policy = "chargescooter:api")]
    private async Task<IResult> HideScooter(HttpContext context,
        [FromServices] IMapper mapper, 
        [FromBody] ScooterIdViewModel viewModel)
    {
        var id = context.User.GetId();
        Console.WriteLine($"--> User with id: {id} has hidden scooter with id:{viewModel.ScooterId}");

        return await _scooterManager.HideScooter(viewModel);
    }
    
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Policy = "chargescooter:api")]
    private async Task<IResult> AppearScooter(HttpContext context,
        [FromServices] IMapper mapper, 
        [FromBody] ScooterIdViewModel viewModel)
    {
        var id = context.User.GetId();
        Console.WriteLine($"--> User with id: {id} has appeared scooter with id:{viewModel.ScooterId}");

        return await _scooterManager.AppearScooter(viewModel);
    }
}