using Gateway.Endpoints.ScooterController.ViewModels;

namespace Gateway.Application.Services.ScooterManagers;

public interface IScooterManager
{
    Task<IResult> AddScooter(AddScooterViewModel viewModel);
    Task<IResult> HideScooter(ScooterIdViewModel viewModel);
    Task<IResult> AppearScooter(ScooterIdViewModel viewModel);
}