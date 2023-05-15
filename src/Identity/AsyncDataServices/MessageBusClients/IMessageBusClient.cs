using Identity.Dtos;

namespace Identity.AsyncDataServices.MessageBusClients;

public interface IMessageBusClient
{
    void PublishNewUser(PublishUserDto userPublishDto);
}