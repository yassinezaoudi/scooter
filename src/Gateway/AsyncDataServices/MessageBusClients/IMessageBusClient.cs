using Gateway.Dtos;

namespace Gateway.AsyncDataServices.MessageBusClients;

public interface IMessageBusClient
{
    void CreateNewPassword(RequestPasswordDto requestPasswordDto);
    void AddNewScooter(RequestAddNewScooterDto addNewScooterDto);
}