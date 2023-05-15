using AutoMapper;
using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Application.Services.UserManagers;
using Gateway.Dtos;
using Gateway.Endpoints.Information.ViewModels;
using Gateway.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
namespace Gateway.Endpoints.Information;

public class InformationDefinition : AppDefinition
{
    public override void ConfigureApplication(WebApplication app)
    {
        app.MapGet("~/info", GetInfoAsync).ExcludeFromDescription();
        app.MapPut("~/info", PutInfoAsync).ExcludeFromDescription();
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    private async Task<UserInfoDto> GetInfoAsync(HttpContext context, 
        [FromServices] IUserManager userManager, 
        [FromServices] IMapper mapper)
    {
        var id = context.User.GetId();

        System.Console.WriteLine("--> GetInfoAsync User Id: " + id);

        var user = await userManager.GetUser(id);
        if (user is null)
        {
            System.Console.WriteLine("User is null. User can not be found in userManager");
        }
        return mapper.Map<UserInfoDto>(user);
    }
    
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Policy = "customer:api")]
    private async Task<IResult> PutInfoAsync(HttpContext context, 
        [FromServices] IUserManager userManager, 
        [FromServices] IMapper mapper, 
        [FromBody] UserInfoViewModel viewModel)
    {
        var id = context.User.GetId();
        var user = await userManager.GetUser(id);
        
        if (user is null)
        {
            return Results.BadRequest($"User id does not exists: {id}");
        }
        
        user.Email = viewModel.Email;
        user.Name = viewModel.Name;

        await userManager.UpdateUserInfo(user);
        
        var userInfoDto = mapper.Map<UserInfoDto>(user);
        
        return Results.Ok(userInfoDto);
    }
}