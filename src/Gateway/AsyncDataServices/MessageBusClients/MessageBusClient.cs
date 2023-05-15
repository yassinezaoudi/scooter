using System.Text;
using System.Text.Json;
using Gateway.Dtos;
using RabbitMQ.Client;

namespace Gateway.AsyncDataServices.MessageBusClients;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQHost"),
            Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQPort")!)
        };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();


            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;

            Console.WriteLine("--> Connected to MessageBus");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus {ex.Message}");
        }
    }

    private void SendMessage(string message, string exchange, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        //_channel.ExchangeDeclare(exchange: "requestPassword", type: ExchangeType.Direct);
        _channel.BasicPublish(exchange: exchange,
            routingKey: routingKey,
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

    public void CreateNewPassword(RequestPasswordDto requestPasswordDto)
    {
        var message = JsonSerializer.Serialize(requestPasswordDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");

            SendMessage(message, "passwordRequested", "password");
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection is Closed, not sending");
        }
    }

    public void AddNewScooter(RequestAddNewScooterDto addNewScooterDto)
    {
        var message = JsonSerializer.Serialize(addNewScooterDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabbitMQ Connection Open, sending message...");

            SendMessage(message, "scooterController", "scooterController");
        }
        else
        {
            Console.WriteLine("--> RabbitMQ Connection is Closed, not sending");
        }
    }
}