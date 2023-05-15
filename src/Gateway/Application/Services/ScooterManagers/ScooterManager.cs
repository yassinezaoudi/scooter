using System.Text;
using System.Text.Json;
using AutoMapper;
using Gateway.AsyncDataServices.MessageBusClients;
using Gateway.Dtos;
using Gateway.Endpoints.ScooterController.ViewModels;

namespace Gateway.Application.Services.ScooterManagers;

public class ScooterManager : IScooterManager
{
    private readonly IMessageBusClient _client;
    private readonly IMapper _mapper;
    private readonly HttpClient _httpClient;

    public ScooterManager(IMessageBusClient client, IMapper mapper, HttpClient httpClient)
    {
        _client = client;
        _mapper = mapper;
        _httpClient = httpClient;
    }
    public async Task<IResult> AddScooter(AddScooterViewModel viewModel)
    {
        var requestAddNewScooter = _mapper.Map<RequestAddNewScooterDto>(viewModel);

        var uri = "addscooter";
            
        var message = JsonSerializer.Serialize(requestAddNewScooter);

        return await SendMessage(uri, message);
    }
    
    public async Task<IResult> HideScooter(ScooterIdViewModel viewModel)
    {
        var uri = "hide";
            
        var message = JsonSerializer.Serialize(viewModel);

        return await SendMessage(uri, message);
    }

    public async Task<IResult> AppearScooter(ScooterIdViewModel viewModel)
    {
        var uri = "appear";
            
        var message = JsonSerializer.Serialize(viewModel);

        return await SendMessage(uri, message);
    }

    private async Task<IResult> SendMessage(string uri, string message)
    {
        try
        {
            uri = _httpClient.BaseAddress + uri;
        
            var result = await _httpClient.PostAsync(uri, 
                new StringContent(message, Encoding.UTF8, "application/json"));
        
            if (result.IsSuccessStatusCode)
            {
                var output = await result.Content.ReadAsStringAsync();
                return Results.Ok(output);
            }

            return Results.Problem();
        }
        catch (Exception e)
        {
            Console.WriteLine("--> ScooterManager: Exception: " + e);
            return Results.BadRequest("--> Add New Scooter something happened...");
        }
    }
}