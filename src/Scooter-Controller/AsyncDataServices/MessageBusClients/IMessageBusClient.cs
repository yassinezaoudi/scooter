using Scooter_Controller.Dtos;

namespace Scooter_Controller.AsyncDataServices.MessageBusClients;

public interface IMessageBusClient
{
    void PublishNewScooter(PublishNewScooterDto scooterPublishedDto);
    void UpdateScooterState(ScooterUpdateStateDto scooterUpdateStateDto);
}