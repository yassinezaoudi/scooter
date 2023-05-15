using AutoMapper;
using Coravel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scooter_Controller.Application.Services.EventProcessing;
using Scooter_Controller.Application.Services.ScooterSpaceTimeInfo;
using Scooter_Controller.Application.Services.ScooterSpaceTimeInfo.ScooterInfoRequests;
using Scooter_Controller.AsyncDataServices;
using Scooter_Controller.AsyncDataServices.MessageBusClients;
using Scooter_Controller.Database;
using Scooter_Controller.Dtos;
using Scooter_Controller.Models;
using Scooter_Controller.Models.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddTransient<IScooterInfoRequester, ScooterInfoRequester>();

builder.Services.AddScheduler();
builder.Services.AddTransient<SpaceTimeCollector>();

var app = builder.Build();

// getting info from scooters each 10 sec
app.Services.UseScheduler(scheduler =>
{
    var jobSchedule = scheduler.Schedule<SpaceTimeCollector>();
    jobSchedule.EverySeconds(10);
});

// start rent: scooter is starting being rent, is not available to rent by anyone else.
app.MapPost("/start", StartRent).ExcludeFromDescription();

// stop rent: current rent is stopped and scooter is free to rent.
app.MapPost("/stop", StopRent).ExcludeFromDescription();

app.MapPost("/hide", HideFromRent).ExcludeFromDescription();

app.MapPost("/appear", AppearToRent).ExcludeFromDescription();

app.MapPost("/addscooter", AddScooter).ExcludeFromDescription();

app.Run();

async Task<IResult> HideFromRent([FromServices] AppDbContext context,
    [FromServices] IScooterInfoRequester infoRequester,
    [FromServices] IMessageBusClient client,
    [FromBody] ScooterIdViewModel viewModel, bool shouldBeOpened = false) // by default this method closes scooter
{
    try
    {
        var scooter = await context.Scooters.FirstOrDefaultAsync(t => t.Id == viewModel.ScooterId);

        if (scooter is null)
        {
            System.Console.WriteLine($"--> Scooter with this id does not exists. {viewModel.ScooterId}");

            return Results.BadRequest(new RequestResultDto());
        }
        
        scooter.IsAvailableToRent = shouldBeOpened;
        var spaceTime = infoRequester.GetScooterInfo(scooter);

        client.UpdateScooterState(new ScooterUpdateStateDto(spaceTime));

        await context.SaveChangesAsync();

        return Results.Ok(new RequestResultDto(isSuccess: true, spaceTime));
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return Results.BadRequest(e.Message);
    }

}

async Task<IResult> AppearToRent([FromServices] AppDbContext context,
    [FromServices] IScooterInfoRequester infoRequester,
    [FromServices] IMessageBusClient client,
    [FromBody] ScooterIdViewModel viewModel)
{
    return await HideFromRent(context, infoRequester, client, viewModel, true);
}

async Task<IResult> StartRent([FromServices] AppDbContext context,
    [FromServices] IScooterInfoRequester infoRequester,
    [FromServices] IMessageBusClient client,
    [FromBody] ScooterIdViewModel viewModel)
{
    // send request to scooter to be ready for ride

    return await HideFromRent(context, infoRequester, client, viewModel, false);
}

async Task<IResult> StopRent([FromServices] AppDbContext context,
    [FromServices] IScooterInfoRequester infoRequester,
    [FromServices] IMessageBusClient client,
    [FromBody] ScooterIdViewModel viewModel)
{
    // send request to scooter to block it from riding

    return await HideFromRent(context, infoRequester, client, viewModel, true);
}

async Task<IResult> AddScooter([FromServices] AppDbContext context,
    [FromServices] IMessageBusClient busClient,
    [FromServices] IMapper mapper,
    [FromServices] IScooterInfoRequester infoRequester,
    [FromBody] AddNewScooterDto addNewScooterDto)
{
    try
    {
        var scooter = mapper.Map<Scooter>(addNewScooterDto);
        context.Scooters.Add(scooter);

        System.Console.WriteLine($"--> Scooter: {scooter.Id}");
        var spaceTime = infoRequester.GetScooterInfo(scooter);
        context.SpaceTimes.Add(spaceTime);
        await context.SaveChangesAsync();

        var scooterPublishedDto = new PublishNewScooterDto(spaceTime);

        busClient.PublishNewScooter(scooterPublishedDto);

        return Results.Ok(scooter.Id);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Exception: {ex.Message}");
        return Results.BadRequest(ex.Message);
    }

}