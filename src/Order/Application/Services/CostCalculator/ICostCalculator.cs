namespace Order.Application.Services.CostCalculator;

public interface ICostCalculator
{
    int CalculateFinalSum(long unixStartTime, long unixFinishTime);
}