namespace Order.Models;

public class Rent
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int ScooterId { get; set; }
    public long UnixStartTime { get; set; }
    public long? UnixFinishTime { get; set; }
    public double LatitudeStart { get; set; }
    public double LongitudeStart { get; set; }
    public double? LatitudeFinish { get; set; }
    public double? LongitudeFinish { get; set; }
    public int? FinalCost { get; set; }
    public int PaymentId { get; set; }
    // TODO check if rentstates work
    public RentStates State { get; set; }
}

public enum RentStates
{
    Started,
    Finished,
    Paid
}