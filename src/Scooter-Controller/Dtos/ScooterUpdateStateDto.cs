using Scooter_Controller.Models;

namespace Scooter_Controller.Dtos;

public class ScooterUpdateStateDto : GenericEventDto
{
    public int Id { get; set; }
    public bool ShouldBeOpened { get; set; } 
    public int PowerPercentage { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }

    public ScooterUpdateStateDto(SpaceTime spaceTime)
    {
        Id = spaceTime.Scooter.Id;
        ShouldBeOpened = spaceTime.Scooter.IsAvailableToRent;
        PowerPercentage = spaceTime.PowerPercentage;
        Latitude = spaceTime.Latitude;
        Longtitude = spaceTime.Longtitude;
        Event = "ScooterUpdateState";
    }
}