using AutoMapper;
using FreeScooters.Application.Services.EventProcessing;
using FreeScooters.AsyncDataServices;
using FreeScooters.Database;
using FreeScooters.Dtos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

var app = builder.Build();

app.MapGet("/scooters", (AppDbContext context, IMapper mapper) =>
{
    var freeScootersDto = context.Scooters.Where(s => s.IsOpened).Select(s => mapper.Map<FreeScooterInfoDto>(s));
    System.Console.WriteLine(freeScootersDto.Count());
    return Results.Ok(freeScootersDto);
});

app.Run();
