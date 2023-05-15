using System.Text.Json;
using Password_Generator.Application.Services.CodeSending;
using Password_Generator.AsyncDataServices.MessageBusClients;
using Password_Generator.Database;
using Password_Generator.Dtos;
using Password_Generator.Models;

namespace Password_Generator.Application.Services.EventProcessing;

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
            case EventType.PasswordRequested:
                await CreatePassword(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining Event");

        System.Console.WriteLine("!-> notificationMessage: " + notificationMessage);

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        if (eventType is null)
        {
            Console.WriteLine("--> Error while getting an event");
            return EventType.Undertermined;
        }

        switch (eventType.Event)
        {
            case "Password_Requested":
                Console.WriteLine("--> Password Requested Event Detected");
                return EventType.PasswordRequested;
            default:
                Console.WriteLine("--> Could not determine the event type");
                return EventType.Undertermined;
        }
    }

    private async Task CreatePassword(string passwordRequestedMessage)
    {
        using var scope = _scopeFactory.CreateScope();

        var random = scope.ServiceProvider.GetRequiredService<Random>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var busClient = scope.ServiceProvider.GetRequiredService<IMessageBusClient>();
        var codeSender = scope.ServiceProvider.GetRequiredService<ICodeSender>();

        var passwordRequestedDto = JsonSerializer
            .Deserialize<PasswordRequestedDto>(passwordRequestedMessage);

        if (passwordRequestedDto is null)
        {
            throw new ArgumentNullException(
                "No object was sent to create password via rabbitMQ + gateway");
        }
        // uncomment when needs a call from service with code
        var code = await codeSender.SendCode(passwordRequestedDto.PhoneNumber);

        if (code is null)
        {
           System.Console.WriteLine("--> Code returned null");
           return;
        }
        var password = new OneTimePassword(passwordRequestedDto.PhoneNumber, code);
        // var password = new OneTimePassword(passwordRequestedDto.PhoneNumber, random, 4);

        context.OneTimePasswords.Add(password);
        await context.SaveChangesAsync();

        var passwordPublishedDto = new PasswordPublishedDto(password, "Password_Created");
        busClient.PublishNewPassword(passwordPublishedDto);
    }
}

enum EventType
{
    PasswordRequested,
    Undertermined
}