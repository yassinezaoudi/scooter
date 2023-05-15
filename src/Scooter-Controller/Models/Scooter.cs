namespace Scooter_Controller.Models;

public class Scooter
{
    public int Id { get; set; }
    public string Model { get; set; }
    public bool IsAvailableToRent { get; set; }
    public string LinkToScooter { get; set; }
}