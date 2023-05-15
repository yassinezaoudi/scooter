namespace Scooter_Controller.Models;

public class SpaceTime
{
    public Guid Id { get; set; }
    public Scooter Scooter { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
    public int PowerPercentage { get; set; }
    public long UnixTime { get; set; }

    public SpaceTime()
    {

    }
}