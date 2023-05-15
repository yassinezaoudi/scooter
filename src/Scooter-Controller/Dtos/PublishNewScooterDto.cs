using Scooter_Controller.Models;

namespace Scooter_Controller.Dtos;

public class PublishNewScooterDto : GenericEventDto
{
    public int Id { get; set; }
    public string Model { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
    public int PowerPercentage { get; set; }

    public PublishNewScooterDto(SpaceTime spaceTime)
    {
        Id = spaceTime.Scooter.Id;
        Model = spaceTime.Scooter.Model;
        Latitude = spaceTime.Latitude;
        Longtitude = spaceTime.Longtitude;
        PowerPercentage = spaceTime.PowerPercentage;
        Event = "AddNewScooter";
    }
}