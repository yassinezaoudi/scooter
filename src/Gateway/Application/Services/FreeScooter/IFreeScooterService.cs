namespace Gateway.Application.Services.FreeScooter;

public interface IFreeScooterService
{
    Task<IResult> GetFreeScooters();
}