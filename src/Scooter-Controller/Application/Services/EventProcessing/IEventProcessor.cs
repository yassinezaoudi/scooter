namespace Scooter_Controller.Application.Services.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}