using AutoMapper;
using Gateway.AsyncDataServices.MessageBusClients;
using Gateway.Dtos;

namespace Gateway.Application.Services.PasswordService;

public class PasswordService : IPasswordService
{
    private readonly IMessageBusClient _client;
    private readonly IMapper _mapper;

    public PasswordService(IMessageBusClient client, IMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }
    public async Task<IResult> GeneratePassword(PasswordGeneratorDto passwordGeneratorDto, string ip)
    {
        try
        {
            var passwordRequestedDto = _mapper.Map<RequestPasswordDto>(passwordGeneratorDto);
            passwordRequestedDto.Ip = ip;
            _client.CreateNewPassword(passwordRequestedDto);

            return Results.Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Results.Problem("--> Generate Password something happened...");
        }
    }
}