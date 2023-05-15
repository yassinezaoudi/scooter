namespace Password_Generator.Application.Services.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}