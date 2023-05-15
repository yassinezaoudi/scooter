namespace Gateway.Application.Services.FreeScooter;

public class FreeScooterService : IFreeScooterService
{
    private readonly HttpClient _httpCLient;

    public FreeScooterService(HttpClient httpClient)
    {
        _httpCLient = httpClient;
    }
    public async Task<IResult> GetFreeScooters()
    {
        var uri = _httpCLient.BaseAddress + "scooters";

        var response = await _httpCLient.GetAsync(uri);

        if (response.IsSuccessStatusCode)
        {
            return Results.Ok(await response.Content.ReadAsStringAsync());
        }
        else
        {
            return Results.BadRequest();
        }
    }
}