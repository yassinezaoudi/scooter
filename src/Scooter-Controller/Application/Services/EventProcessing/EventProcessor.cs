using System.Text.Json;
using AutoMapper;
using Scooter_Controller.Application.Services.ScooterSpaceTimeInfo.ScooterInfoRequests;
using Scooter_Controller.AsyncDataServices.MessageBusClients;
using Scooter_Controller.Database;
using Scooter_Controller.Dtos;
using Scooter_Controller.Models;

namespace Scooter_Controller.Application.Services.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.AddNewScooter:
                await AddScooter(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType is null)
        {
            System.Console.WriteLine("--> Error while getting an event");
            return EventType.Undertermined;
        }

        switch (eventType.Event)
        {
            case "AddNewScooter":
                Console.WriteLine("--> Add New Scooter Event Detected");
                return EventType.AddNewScooter;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undertermined;
        }
    }

    private async Task AddScooter(string addNewScooterMessage)
    {
        System.Console.WriteLine($"--> Message: {addNewScooterMessage}");
        using var scope = _scopeFactory.CreateScope();
        System.Console.WriteLine("--> Before getting context");
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        System.Console.WriteLine("--> After context");
        var busClient = scope.ServiceProvider.GetRequiredService<IMessageBusClient>();
        var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
        var infoRequester = scope.ServiceProvider.GetRequiredService<IScooterInfoRequester>();

        var addNewScooterDto = JsonSerializer
            .Deserialize<AddNewScooterDto>(addNewScooterMessage);
        System.Console.WriteLine($"--> Model: {addNewScooterDto.Model} | Link: {addNewScooterDto.LinkToScooter}");
        var scooter = mapper.Map<Scooter>(addNewScooterDto);
        context.Scooters.Add(scooter);

        System.Console.WriteLine($"--> Scooter: {scooter.Id}");
        var spaceTime = infoRequester.GetScooterInfo(scooter);
        context.SpaceTimes.Add(spaceTime);
        
        var scooterPublishedDto = new PublishNewScooterDto(spaceTime);
        
        busClient.PublishNewScooter(scooterPublishedDto);
        await context.SaveChangesAsync();
    }
}

enum EventType
{
    AddNewScooter,
    Undertermined
}