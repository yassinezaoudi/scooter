namespace FreeScooters.Application.Services.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}