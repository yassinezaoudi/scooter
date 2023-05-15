namespace FreeScooters.Dtos;

public class AddNewScooterDto
{
    public int Id { get; set; }
    public string Model { get; set; }
    public double Latitude { get; set; }
    public double Longtitude { get; set; }
    public int PowerPercentage { get; set; }
}