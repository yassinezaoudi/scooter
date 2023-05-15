using System.Text;
using System.Text.Json;
using Order.Application.Services.ScooterManager.ViewModels;

namespace Order.Application.Services.ScooterManager;

public class ScooterManager : IScooterManager
{
    private readonly HttpClient _httpClient;

    public ScooterManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ScooterResponseViewModel> AppearScooter(int scooterId)
    {
        var uri = "appear";

        return await SendMessage(scooterId, uri);
    }

    public async Task<ScooterResponseViewModel> HideScooter(int scooterId)
    {
        var uri = "hide";

        return await SendMessage(scooterId, uri);
    }

    public async Task<ScooterResponseViewModel> StartRent(int scooterId)
    {
        var uri = "start";

        return await SendMessage(scooterId, uri);

    }

    public async Task<ScooterResponseViewModel> StopRent(int scooterId)
    {
        var uri = "stop";

        return await SendMessage(scooterId, uri);
    }

    private async Task<ScooterResponseViewModel> SendMessage(int scooterId, string uri)
    {
        try
        {
            var scooterDto = new ScooterDto
            {
                ScooterId = scooterId
            };
            var message = JsonSerializer.Serialize(scooterDto);
            uri = _httpClient.BaseAddress + uri;
            System.Console.WriteLine("SRVM--> " + message);
            System.Console.WriteLine("SRVM-->" + uri);

            var result = await _httpClient.PostAsync(uri,
                new StringContent(message, Encoding.UTF8, "application/json"));

            var output = await result.Content.ReadAsStringAsync();

            System.Console.WriteLine("SRVM--> " + result.IsSuccessStatusCode);

            System.Console.WriteLine("SRVM--> " + output);

            var scooterResponseViewModel = JsonSerializer.Deserialize<ScooterResponseViewModel>(output);
            if (scooterResponseViewModel is null)
            {
                System.Console.WriteLine("scooterResponseViewModel is null. It can not convert from json");
            }
            System.Console.WriteLine("SRVM-->" + scooterResponseViewModel.isSuccess);
            System.Console.WriteLine("SRVM-->" + scooterResponseViewModel.latitude);
            System.Console.WriteLine("SRVM-->" + scooterResponseViewModel.longitude);
            return scooterResponseViewModel;
        }
        catch (Exception e)
        {
            Console.WriteLine("--> ScooterManager: Exception: " + e);
            System.Console.WriteLine($"--> Could not Send Message to Scooter Controller");
            return new ScooterResponseViewModel
            {
                isSuccess = false,
                latitude = 0,
                longitude = 0
            };
        }
    }

    private class ScooterDto
    {
        public int ScooterId { get; set; }
    }
}