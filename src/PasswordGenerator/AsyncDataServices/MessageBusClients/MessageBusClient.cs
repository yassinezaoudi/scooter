using System.Text;
using System.Text.Json;
using Password_Generator.Dtos;
using RabbitMQ.Client;

namespace Password_Generator.AsyncDataServices.MessageBusClients;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient()
    {
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQHost"),
            Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQPort")!)
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: "passwordCreated", type: ExchangeType.Direct);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

            Console.WriteLine("--> Connected to MessageBus");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus {ex.Message}");
        }
    }

    public void PublishNewPassword(PasswordPublishedDto passwordPublishedDto)
    {
        var message = JsonSerializer.Serialize(passwordPublishedDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");

            SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection is Closed, not sending");
        }
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "passwordCreated",
                        routingKey: "password",
                        basicProperties: null,
                        body: body);

        Console.WriteLine($"--> We have send {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("--> MessageBus Disposed");

        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }
}