using System.Text;
using Gateway.Application.Services.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Gateway.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

    public MessageBusSubscriber(
        IConfiguration configuration,
        IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = Environment.GetEnvironmentVariable("RabbitMQHost"),
            Port = int.Parse(Environment.GetEnvironmentVariable("RabbitMQPort")!)
        };
        // factory.UserName = "guest";
        // factory.Password = "guest";
        System.Console.WriteLine($"---------> {Environment.GetEnvironmentVariable("RabbitMQHost")}\n" +
            $"{Environment.GetEnvironmentVariable("RabbitMQPort")}");
        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "userCreated", type: ExchangeType.Direct);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(
            queue: _queueName,
            exchange: "userCreated",
            routingKey: "user");

        System.Console.WriteLine("--> Listening on the Message Bus...");

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        System.Console.WriteLine("--> Connection to the Message Bus Shutdown");
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (ModuleHandle, ea) =>
        {
            System.Console.WriteLine("--> Event Received!");

            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            _eventProcessor.ProcessEvent(notificationMessage);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}