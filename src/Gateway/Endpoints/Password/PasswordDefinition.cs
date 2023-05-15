using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Application.Services.PasswordService;
using Gateway.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Endpoints.Password;

public class PasswordDefinition : AppDefinition
{
    private IPasswordService? _passwordService;


    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddScoped<IPasswordService, PasswordService>();
    }
    
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapPost("/password", GeneratePassword).ExcludeFromDescription();
        _passwordService = app.Services.GetService<IPasswordService>();

        Console.WriteLine("--> Password Map Post Detected");
    }

    private async Task<IResult> GeneratePassword([FromBody] PasswordGeneratorDto passwordGeneratorDto,
        HttpContext context)
    {
        if (_passwordService is null)
        {
            throw new Exception("--> PasswordService is null");
        }
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp is not null)
        {
            try
            {
                var clientIp = remoteIp.MapToIPv4().ToString();
                return await _passwordService.GeneratePassword(passwordGeneratorDto, clientIp);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("--> Can not map to ipv4");
                System.Console.WriteLine(ex.Message);
            }
        }

        return Results.BadRequest("Ip address can not be seen");
    }
}