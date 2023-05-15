using System.Text.Json;
using FreeScooters.Database;
using FreeScooters.Dtos;
using FreeScooters.Models;
using Microsoft.EntityFrameworkCore;

namespace FreeScooters.Application.Services.EventProcessing;

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
            case EventType.ScooterUpdateState:
                await UpdateScooterState(message);
                break;
            case EventType.AddNewScooter:
                await AddNewScooter(message);
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
            Console.WriteLine("--> Error while getting an event");
            return EventType.Undertermined;
        }

        switch (eventType.Event)
        {
            case "ScooterUpdateState":
                Console.WriteLine("--> Scooter Update State Event Detected");
                return EventType.ScooterUpdateState;
            case "AddNewScooter":
                Console.WriteLine("--> Add New Scooter Event Detected");
                return EventType.AddNewScooter;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undertermined;
        }
    }

    private async Task UpdateScooterState(string scooterMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var scooterDto = JsonSerializer
            .Deserialize<ScooterUpdateStateDto>(scooterMessage);

        if (scooterDto is not null)
        {
            var scooter = await context.Scooters.FirstOrDefaultAsync(scooter => scooter.Id == scooterDto.Id);

            if (scooter is not null)
            {
                scooter.UpdateState(scooterDto);

                await context.SaveChangesAsync();
            }
        }

    }

    private async Task AddNewScooter(string scooterMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        var scooterDto = JsonSerializer
            .Deserialize<AddNewScooterDto>(scooterMessage);
        
        if (scooterDto is not null)
        {
            var scooter = new Scooter(scooterDto);

            context.Scooters.Add(scooter);

            await context.SaveChangesAsync();
        }
        
    }
}

enum EventType
{
    ScooterUpdateState,
    AddNewScooter,
    Undertermined
}