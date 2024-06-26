using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services;

public class RabbitMQClientService : IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;
    public static string ExchangeName = "ImageDirectExchange";
    public static string RoutingWatermark = "watermark-route-image";
    public static string QueueName = "queue-watermark-image";

    private readonly ILogger<RabbitMQClientService> _logger;

    public RabbitMQClientService(ConnectionFactory connectionFactory,ILogger<RabbitMQClientService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        Connect(); //ilk nesne örneği alındığında bağlantı kurulsun
    }

    public IModel Connect()
    {
        _connectionFactory.DispatchConsumersAsync = true;
        _connection = _connectionFactory.CreateConnection();
        if (_channel is { IsOpen: true })
        {
            return _channel;
        }

        _channel = _connection.CreateModel(); //connectiondan 1 tane channel oluşturduk
        _channel.ExchangeDeclare(ExchangeName,type:"direct",true,false);
        _channel.QueueDeclare(QueueName, true, false, false, null);
        
        _channel.QueueBind(exchange:ExchangeName,queue:QueueName,routingKey:RoutingWatermark);
        
        _logger.LogInformation("RabbitMQ ile bağlantı kuruldu");

        return _channel;
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();
        _channel = default;
        _connection?.Close();
        _connection?.Dispose();
        
        _logger.LogInformation("RabbitMQ ile bağlantı koptu...");
    }
}