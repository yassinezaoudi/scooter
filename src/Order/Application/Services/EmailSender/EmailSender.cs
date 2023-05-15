namespace Order.Application.Services.EmailSender;

public class EmailSender : IEmailSender
{
    private readonly HttpClient _httpClient;

    public EmailSender(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<IResult> SendEmail(string message, Guid userId)
    {
        // TODO: request to email sender service
        return Results.Ok();
    }
}