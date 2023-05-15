using Password_Generator.Dtos;

namespace Password_Generator.AsyncDataServices.MessageBusClients;

public interface IMessageBusClient
{
    void PublishNewPassword(PasswordPublishedDto passwordPublishedDto);
}