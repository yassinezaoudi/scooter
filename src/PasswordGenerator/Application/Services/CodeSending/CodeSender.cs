using Newtonsoft.Json;

namespace Password_Generator.Application.Services.CodeSending;

public class CodeSender : ICodeSender
{
    private readonly HttpClient _httpClient;

    public CodeSender(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> SendCode(string phoneNumber)
    {

        var requestUri = _httpClient.BaseAddress
            + $"/get_code/" 
            + $"{phoneNumber}";
        System.Console.WriteLine(requestUri);
        var response = await _httpClient.GetStringAsync(requestUri);
        System.Console.WriteLine(response);

        dynamic result = JsonConvert.DeserializeObject(response);



        if (result != null)
        {
            bool status = result.success;
            string code = result.data.code;
            System.Console.WriteLine($"--> {status} | {code}");
            if (status)
            {
                return (string)code;
            }
        }

        return null;
    }
}