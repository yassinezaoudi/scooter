using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.Application.Services.CostCalculator;
using Order.Application.Services.EmailSender;
using Order.Application.Services.PaymentService;
using Order.Application.Services.ScooterManager;
using Order.Database;
using Order.Dtos;
using Order.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
});



builder.Services.AddHttpClient<IPaymentService, PaymentService>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HttpPaymentService")!);
});

builder.Services.AddHttpClient<IEmailSender, EmailSender>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HttpEmailSender")!);
});

builder.Services.AddHttpClient<IScooterManager, ScooterManager>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("HttpScooterManager")!);
});

builder.Services.AddTransient<ICostCalculator, CostCalculator>();

// TODO Environments: HttpScooterManager, HttpEmailSender, HttpPaymentService, DB_CONNECTION_STRING, MoneyToHold

var app = builder.Build();

app.MapPost("/start", async (
    [FromServices] IScooterManager scooterManager,
    [FromServices] IPaymentService paymentService,
    [FromServices] AppDbContext context,
    [FromBody] RentDto startRentDto) =>
{
    var moneyToHold = int.Parse(Environment.GetEnvironmentVariable("MoneyToHold")!);
    var scooterRequestResult = await scooterManager.HideScooter(startRentDto.ScooterId);

    System.Console.WriteLine(scooterRequestResult.isSuccess);
    System.Console.WriteLine(scooterRequestResult.latitude);
    System.Console.WriteLine(scooterRequestResult.longitude);

    if (scooterRequestResult.isSuccess is not true)
    {
        System.Console.WriteLine($"--> Could not hide scooter {startRentDto.ScooterId} "
            + $"| UserId = {startRentDto.UserId}");
        return Results.BadRequest($"Could not hide scooter {startRentDto.ScooterId} "
            + $"| UserId = {startRentDto.UserId}");
    }

    var paymentResult = await paymentService.HoldMoney(moneyToHold, startRentDto.UserId);

    if (!paymentResult.IsSuccess)
    {
        System.Console.WriteLine($"--> Could not hold money {startRentDto.ScooterId} "
            + $"| UserId = {startRentDto.UserId}");


        // откат
        await scooterManager.AppearScooter(startRentDto.ScooterId);

        return Results.BadRequest($"Could not hold money {startRentDto.ScooterId} "
            + $"| UserId = {startRentDto.UserId}");
    }

    scooterRequestResult = await scooterManager.StartRent(startRentDto.ScooterId);

    if (scooterRequestResult.isSuccess is not true)
    {
        System.Console.WriteLine($"--> Could not start renting {startRentDto.ScooterId} "
            + $"| UserId = {startRentDto.UserId}");

        // откат
        await scooterManager.AppearScooter(startRentDto.ScooterId);
        await paymentService.UnHoldMoney(paymentResult.PaymentId);

        return Results.BadRequest($"Could not start renting {startRentDto.ScooterId} "
            + $"| UserId = {startRentDto.UserId}");
    }

    var rent = new Rent
    {
        LatitudeStart = scooterRequestResult.latitude,
        LongitudeStart = scooterRequestResult.longitude,
        UnixStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        UserId = startRentDto.UserId,
        ScooterId = startRentDto.ScooterId,
        State = RentStates.Started,
        PaymentId = paymentResult.PaymentId
    };

    context.Rents.Add(rent);

    await context.SaveChangesAsync();

    return Results.Ok(rent.Id);

});

app.MapPost("/stop", async (
    [FromServices] IScooterManager scooterManager,
    [FromServices] IPaymentService paymentService,
    [FromServices] AppDbContext context,
    [FromServices] ICostCalculator costCalculator,
    [FromBody] RentDto stopRentDto) =>
{
    // остановка самоката

    var unixFinishTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    var scooterRequestResult = await scooterManager.StopRent(stopRentDto.ScooterId);

    if (scooterRequestResult.isSuccess is not true)
    {
        System.Console.WriteLine($"--> Could not stop renting (SC) {stopRentDto.ScooterId} "
            + $"| UserId = {stopRentDto.UserId}");

        return Results.BadRequest($"Could not stop renting (SC) {stopRentDto.ScooterId} "
            + $"| UserId = {stopRentDto.UserId}");
    }

    var rent = await context.Rents.FirstOrDefaultAsync(
        r => r.ScooterId == stopRentDto.ScooterId
            && r.UserId == stopRentDto.UserId
            && r.State == RentStates.Started);

    if (rent is null)
    {
        System.Console.WriteLine("--> Could not find rent with this"
            + $" UserId: {stopRentDto.UserId} and ScooterId:{stopRentDto.ScooterId}");
        return Results.Problem("Could not find rent with this"
            + $" UserId: {stopRentDto.UserId} and ScooterId:{stopRentDto.ScooterId}");
    }

    var finalCost = costCalculator.CalculateFinalSum(rent.UnixStartTime, unixFinishTime);

    rent.FinalCost = finalCost;
    rent.State = RentStates.Finished;
    rent.UnixFinishTime = unixFinishTime;
    rent.LatitudeFinish = scooterRequestResult.latitude;
    rent.LongitudeFinish = scooterRequestResult.longitude;

    await context.SaveChangesAsync();

    // списание средств
    var paymentResult = await paymentService.WithdrawFunds(rent.PaymentId, finalCost);
    if (!paymentResult)
    {
        // делать что-то с клиентом, списание окончательное не произошло
        System.Console.WriteLine($"--> Could not withdraw payment from UserId: {rent.UserId}");
        Results.Problem($"Could not withdraw payment from UserId: {rent.UserId}");
    }

    rent.State = RentStates.Paid;
    await context.SaveChangesAsync();
    return Results.Ok(rent.FinalCost);
});

app.Run();
