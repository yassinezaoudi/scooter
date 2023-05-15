namespace FreeScooters.Dtos;

public class ScooterUpdateStateDto
{
    public int Id { get; set; }
    public bool ShouldBeOpened { get; set; } 
    public int PowerPercentage { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
}