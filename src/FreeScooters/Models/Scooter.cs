using FreeScooters.Dtos;

namespace FreeScooters.Models;

public class Scooter
{
    public int Id { get; set; }
    public string Model { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
    public int PowerPercentage { get; set; }
    public bool IsOpened { get; set; }

    public void UpdateState(ScooterUpdateStateDto scooterUpdateStateDto)
    {
        this.Latitude = scooterUpdateStateDto.Latitude;
        this.Longtitude = scooterUpdateStateDto.Longtitude;
        this.PowerPercentage = scooterUpdateStateDto.PowerPercentage;
        this.IsOpened = scooterUpdateStateDto.ShouldBeOpened;
    }

    public Scooter(AddNewScooterDto scooterDto)
    {
        this.Id = scooterDto.Id;
        this.Model = scooterDto.Model;
        this.Latitude = scooterDto.Latitude;
        this.Longtitude = scooterDto.Longtitude;
        this.PowerPercentage = scooterDto.PowerPercentage;
        this.IsOpened = false;
    }

    public Scooter()
    {
        
    }
}