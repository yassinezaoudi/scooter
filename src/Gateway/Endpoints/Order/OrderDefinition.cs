using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Application.Services.OrderService;
using Gateway.Endpoints.Order.ViewModels;
using Gateway.Endpoints.ScooterController.ViewModels;
using Gateway.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Gateway.Endpoints.Order;

public class OrderDefinition : AppDefinition
{
    private IOrderService? _orderService;


    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddHttpClient<IOrderService, OrderService>(client =>
        {
            client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HttpOrder")!);
        });
    }
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapPost("/start", StartRent).ExcludeFromDescription();
        app.MapPost("/stop", StopRent).ExcludeFromDescription();

        _orderService = app.Services.GetService<IOrderService>();
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        Policy = "customer:api")]
    private async Task<IResult> StopRent(HttpContext context,
        [FromBody] ScooterIdViewModel viewModel)
    {
        var id = context.User.GetId();
        Console.WriteLine($"--> User with id: {id} has stopped rent with ScooterId:{viewModel.ScooterId}");

        return await _orderService.StopRent(new OrderViewModel
        {
            UserId = id,
            ScooterId = viewModel.ScooterId
        });
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        Policy = "customer:api")]
    private async Task<IResult> StartRent(HttpContext context,
        [FromBody] ScooterIdViewModel viewModel)
    {
        var id = context.User.GetId();
        Console.WriteLine($"--> User with id: {id} has started rent with ScooterId:{viewModel.ScooterId}");

        return await _orderService.StartRent(new OrderViewModel
        {
            UserId = id,
            ScooterId = viewModel.ScooterId
        });
    }

}