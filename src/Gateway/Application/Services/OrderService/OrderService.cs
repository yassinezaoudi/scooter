using System.Text;
using System.Text.Json;
using Gateway.Endpoints.Order.ViewModels;

namespace Gateway.Application.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> StartRent(OrderViewModel viewModel)
    {
        var uri = "start";

        var message = JsonSerializer.Serialize(viewModel);

        return await SendMessage(uri, message);
    }

    public async Task<IResult> StopRent(OrderViewModel viewModel)
    {
        var uri = "stop";

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
            Console.WriteLine("--> Order Service: Exception: " + e);
            return Results.BadRequest("--> Ordering something happened...");
        }
    }
}