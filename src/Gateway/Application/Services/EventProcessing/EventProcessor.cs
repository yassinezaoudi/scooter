using System.Text.Json;
using AutoMapper;
using Gateway.Domain;
using Gateway.Dtos;
using Gateway.Infrastructure;

namespace Gateway.Application.Services.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.UserCreated:
                AddUser(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        System.Console.WriteLine("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType is null)
        {
            System.Console.WriteLine("--> Error while getting event");
            return EventType.Undertermined;
        }

        switch (eventType.Event)
        {
            case "User_Created":
                System.Console.WriteLine("--> User Created Published Event Detected");
                return EventType.UserCreated;
            default:
                System.Console.WriteLine("--> Could not determine the event type");
                return EventType.Undertermined;
        }
    }

    private void AddUser(string userRegisteredMessage)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userRepository = context.Users;

        var userRegisteredDto = JsonSerializer.Deserialize<UserRegisteredDto>(userRegisteredMessage);

        try
        {
            var user = _mapper.Map<User>(userRegisteredDto);

            userRepository.Add(user);

            context.SaveChangesAsync();

            Console.WriteLine($"--> User was created: {user.Id} | {user.PhoneNumber}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
        }
    }
}

enum EventType
{
    UserCreated,
    Undertermined
}