using Scooter_Controller.Models;

namespace Scooter_Controller.Application.Services.ScooterSpaceTimeInfo.ScooterInfoRequests;

public class ScooterInfoRequester : IScooterInfoRequester
{
    private readonly Random _random;

    public ScooterInfoRequester()
    {
        _random = new Random();
    }
    public SpaceTime GetScooterInfo(Scooter scooter)
    {
        // request to scooter.linkToScooter
        double latitude = (_random.NextDouble() - 0.5) * 180;
        double longitude = (_random.NextDouble() - 0.5) * 360;
        int power = _random.Next(20, 100);
        return new SpaceTime
        {
            Latitude = latitude,
            Longtitude = longitude,
            Scooter = scooter,
            PowerPercentage = power,
            UnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
}