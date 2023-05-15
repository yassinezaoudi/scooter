using Scooter_Controller.Models;

namespace Scooter_Controller.Application.Services.ScooterSpaceTimeInfo.ScooterInfoRequests;

public interface IScooterInfoRequester
{
    SpaceTime GetScooterInfo(Scooter scooter);
}