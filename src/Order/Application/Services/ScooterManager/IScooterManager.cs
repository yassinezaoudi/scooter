using Order.Application.Services.ScooterManager.ViewModels;

namespace Order.Application.Services.ScooterManager;

public interface IScooterManager
{
    Task<ScooterResponseViewModel> HideScooter(int scooterId);
    Task<ScooterResponseViewModel> AppearScooter(int scooterId);
    Task<ScooterResponseViewModel> StartRent(int scooterId);
    Task<ScooterResponseViewModel> StopRent(int scooterId);
}