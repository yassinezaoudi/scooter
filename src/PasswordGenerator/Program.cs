using Microsoft.EntityFrameworkCore;
using Password_Generator.Database;
using Password_Generator.AsyncDataServices.MessageBusClients;
using Password_Generator.Application.Services.EventProcessing;
using Password_Generator.AsyncDataServices;
using Password_Generator.Application.Services.CodeSending;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(config =>
        config.UseInMemoryDatabase("Password DEMO"));

}
else if (builder.Environment.IsProduction())
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    builder.Services.AddDbContext<AppDbContext>(o =>
        o.UseNpgsql(connectionString));
}

builder.Services.AddHttpClient<ICodeSender, CodeSender>(client =>
{
    client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("CodeSendPath") 
        + Environment.GetEnvironmentVariable("CodeSendToken"));
});

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<Random>();

var app = builder.Build();

var random = new Random();

// app.MapPost("/generate",
//     async ([FromServices] AppDbContext context, [FromServices] IMessageBusClient busClient,
//         [FromBody] PasswordViewModel viewModel) =>
//     {
//         var password = new OneTimePassword(viewModel.PhoneNumber, random);

//         context.OneTimePasswords.Add(password);
//         await context.SaveChangesAsync();

//         var passwordPublishedDto = new PasswordPublishedDto(password, "Password_Created");
//         busClient.PublishNewPassword(passwordPublishedDto);
//         return Results.Ok("The password was generated");
//     });

app.Run();