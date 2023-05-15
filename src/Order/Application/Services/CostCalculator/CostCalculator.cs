namespace Order.Application.Services.CostCalculator;

public class CostCalculator : ICostCalculator
{
    public int CalculateFinalSum(long unixStartTime, long unixFinishTime)
    {
        // todo: calculation of final sum
        return (int)(unixFinishTime - unixStartTime);
    }
}